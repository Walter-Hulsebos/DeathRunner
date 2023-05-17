//System libraries first

//Unity-specific libraries next
using System;
using System.Collections;
using DeathRunner.Attributes;
using HFSM;
using JetBrains.Annotations;
using QFSW.QC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using static Unity.Mathematics.math;

//Third-party libraries next

//Project-specific libraries last
using F32x2 = Unity.Mathematics.float2;

namespace DeathRunner.Player
{
    public sealed class Player : MonoBehaviour
    {
        //NOTE: [Walter] Make shared states possible??

        [SerializeField] private PlayerReferences      playerReferences = new();
        [SerializeField] private PlayerAttributes      playerAttributes = new();

        [Tooltip("Locomotion Settings for Normal-Time")]
        [SerializeField] private LocomotionSettings    locomotionNTSettings;
        
        [Tooltip("Idle Settings for Normal-Time")]
        [SerializeField] private IdleSettings          idleNTSettings;
        [Tooltip("Walk Settings for Normal-Time")]
        [SerializeField] private MoveSettings          moveNtSettings;
        
        [Tooltip("Short Dash Settings for Normal-Time")]
        [SerializeField] private DashShortSettings     dashShortNTSettings;
        [Tooltip("Long Dash Settings for Normal-Time")]
        [SerializeField] private DashLongSettings      dashLongNTSettings;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Combat NT")]
        #endif
        [SerializeField] private MeleeAttackData primaryAttack00NT;
        #if ODIN_INSPECTOR
        [FoldoutGroup("Combat NT")]
        #endif
        [SerializeField] private MeleeAttackData primaryAttack01NT;
        #if ODIN_INSPECTOR
        [FoldoutGroup("Combat NT")]
        #endif
        [SerializeField] private MeleeAttackData primaryAttack02NT;
        
        // [Tooltip("Locomotion Settings for Bullet-Time")]
        // [SerializeField] private LocomotionSettings    locomotionBTSettings;
        //
        // [Tooltip("Idle Settings for Bullet-Time")]
        // [SerializeField] private IdleSettings          idleBTSettings;
        // [Tooltip("Walk Settings for Bullet-Time")]
        // [SerializeField] private MoveSettings          moveBTSettings;
        //
        // #if ODIN_INSPECTOR
        // [FoldoutGroup("Combat BT")]
        // #endif
        // [SerializeField] private MeleeAttack primaryAttack00BT;
        // #if ODIN_INSPECTOR
        // [FoldoutGroup("Combat BT")]
        // #endif
        // [SerializeField] private MeleeAttack primaryAttack01BT;
        // #if ODIN_INSPECTOR
        // [FoldoutGroup("Combat BT")]
        // #endif
        // [SerializeField] private MeleeAttack primaryAttack02BT;

        private State                           _root;
        
        private PlayerState_Alive               _alive;
        private PlayerStateLeaf_Dead            _dead;

        private PlayerState_NormalTime          _normalTime;
        //private PlayerState_BulletTime          _bulletTime;
        
        // Suffixed with "NT" (NormalTime) or "BT" (BulletTime) to avoid name conflicts
        private PlayerState_Locomotion          _locomotionNT;
        
        private PlayerStateLeaf_Idle            _idleNT;
        private PlayerStateLeaf_Move            _walkNT;
        
        private PlayerStateLeaf_DashShort       _dashShortNT;
        private PlayerStateLeaf_DashLong        _dashLongNT;
        
        private PlayerStateLeaf_AttackMelee     _lightAttackNt00;
        private PlayerStateLeaf_AttackMelee     _lightAttackNt01;
        private PlayerStateLeaf_AttackMelee     _lightAttackNt02;
        
        private PlayerStateLeaf_SecondaryAttack _secondaryAttackNT;
        
        // private State                           _locomotionBT;
        //
        // private StateLeaf                       _idleBT;
        // private StateLeaf                       _walkBT;
        //
        // private PlayerStateLeaf_PrimaryAttack   _primaryAttackBT00;
        // private PlayerStateLeaf_PrimaryAttack   _primaryAttackBT01;
        // private PlayerStateLeaf_PrimaryAttack   _primaryAttackBT02;
        //
        // private PlayerStateLeaf_SecondaryAttack _secondaryAttackBT;
        

        #if UNITY_EDITOR
        private void Reset() => playerReferences.Reset(gameObject);

        private void OnValidate() => playerReferences.OnValidate(gameObject);
        #endif
        
        private Boolean HasMoveInput   => any(playerReferences.InputHandler.MoveInput != F32x2.zero);
        private Boolean HasNoMoveInput => !HasMoveInput;
        
        private Boolean HasDashInput   => playerReferences.InputHandler.DashInput;
        
        private void Awake()
        {
            CreateStateTree();
            CreateStateTransitions();
            
            //Initialize root state
            _root.Init();
        }

        private void CreateStateTree()
        {
            _root = new PlayerState_Root
            (/*params child states */
                _alive = new PlayerState_Alive
                (/*params child states */
                    _normalTime = new PlayerState_NormalTime
                    (/*params child states */
                        _locomotionNT = new PlayerState_Locomotion
                        (settings: locomotionNTSettings, references: playerReferences, /*params child states */
                            _idleNT            = new PlayerStateLeaf_Idle(settings: idleNTSettings, references: playerReferences), 
                            _walkNT            = new PlayerStateLeaf_Move(settings: moveNtSettings, references: playerReferences)
                        ), 
                        _dashShortNT     = new PlayerStateLeaf_DashShort(settings: dashShortNTSettings, references: playerReferences), 
                        _dashLongNT      = new PlayerStateLeaf_DashLong( settings: dashLongNTSettings,  references: playerReferences),
                        
                        _lightAttackNt00 = new PlayerStateLeaf_AttackMelee(settings: primaryAttack00NT.Settings, references: playerReferences),
                        _lightAttackNt01 = new PlayerStateLeaf_AttackMelee(settings: primaryAttack01NT.Settings, references: playerReferences),
                        _lightAttackNt02 = new PlayerStateLeaf_AttackMelee(settings: primaryAttack02NT.Settings, references: playerReferences),
                        
                        _secondaryAttackNT = new PlayerStateLeaf_SecondaryAttack()
                    )
                ),
                _dead = new PlayerStateLeaf_Dead()
            );
        }
        
        //TODO: [Walter] InputBuffer/Queue for attacks, if you click the attack button within the timeslot of the previous attack, it will queue the next attack, and execute it after the previous one is finished.
        //Currently, if you click the attack button while the previous attack is still executing, it will cancel the previous attack and execute the new one.

        private void CreateStateTransitions()
        {
            //Alive <-> Dead
            _alive.AddTransition(to: _dead,  conditions: () => playerAttributes.health.IsZero == true);
             _dead.AddTransition(to: _alive, conditions: () => playerAttributes.health.IsZero == false);
            
            //Normal Time <-> Bullet Time
            //For now just use a button press to switch between the two
            //_normalTime.AddTransition(to: _bulletTime, conditions: () => playerReferences.InputHandler.IsSlowMoToggled == true);
            //_bulletTime.AddTransition(to: _normalTime, conditions: () => playerReferences.InputHandler.IsSlowMoToggled == false);

            #region Normal Time Transitions
            
            //IdleNT -> WalkNT
            _idleNT.AddTransition(to: _walkNT, conditions: () => HasMoveInput);
            //WalkNT -> IdleNT
            _walkNT.AddTransition(to: _idleNT, conditions: () => HasNoMoveInput);
            
            //IdleNT -> DashNT
            _idleNT.AddTransition(to: _dashShortNT, conditions: () => HasDashInput);
            //DashNT -> IdleNT
            _dashShortNT.AddTransition(to: _idleNT, conditions: () => HasNoMoveInput && _dashShortNT.IsDoneDashing);

            //WalkNT -> DashNT
            _walkNT.AddTransition(to: _dashShortNT, conditions: () => HasDashInput);
            //DashNT -> WalkNT
            _dashShortNT.AddTransition(to: _walkNT, conditions: () => HasMoveInput && _dashShortNT.IsDoneDashing);
            
            //lightAttackNt00 -> idle
            _lightAttackNt00.AddTransition(to: _idleNT, conditions: () => _lightAttackNt00.IsDoneAttacking && HasNoMoveInput);
            //lightAttackNt00 -> idle
            _lightAttackNt01.AddTransition(to: _idleNT, conditions: () => _lightAttackNt01.IsDoneAttacking && HasNoMoveInput);
            //lightAttackNt00 -> idle
            _lightAttackNt02.AddTransition(to: _idleNT, conditions: () => _lightAttackNt02.IsDoneAttacking && HasNoMoveInput);
            
            //TODO: Add post transitions after attacks when back to walk/idle in which you can still follow up with another attack.
            
            //lightAttackNt00 -> walk
            _lightAttackNt00.AddTransition(to: _walkNT, conditions: () => _lightAttackNt00.IsDoneAttacking && HasMoveInput);
            //lightAttackNt00 -> walk
            _lightAttackNt01.AddTransition(to: _walkNT, conditions: () => _lightAttackNt01.IsDoneAttacking && HasMoveInput);
            //lightAttackNt00 -> walk
            _lightAttackNt02.AddTransition(to: _walkNT, conditions: () => _lightAttackNt02.IsDoneAttacking && HasMoveInput);
            
            //Idle -> PrimaryAttack00
            _idleNT.AddTransition(to: _lightAttackNt00, conditions: () => playerReferences.InputHandler.PrimaryFireInput);
            //Walk -> PrimaryAttack00
            _walkNT.AddTransition(to: _lightAttackNt00, conditions: () => playerReferences.InputHandler.PrimaryFireInput);
            
            //PrimaryAttack00 -> PrimaryAttack01
            _lightAttackNt00.AddTransition(to: _lightAttackNt01, conditions: () => playerReferences.InputHandler.PrimaryFireInput && _lightAttackNt00.CanGoIntoNextAttack);
            
            //PrimaryAttack01 -> PrimaryAttack02
            _lightAttackNt01.AddTransition(to: _lightAttackNt02, conditions: () => playerReferences.InputHandler.PrimaryFireInput && _lightAttackNt01.CanGoIntoNextAttack);
            
            #endregion

            // #region Bullet Time Transitions
            //
            // //Idle -> Walk
            // _idleBT.AddTransition(to: _walkBT, conditions: () => HasMoveInput);
            // //Walk -> Idle
            // _walkBT.AddTransition(to: _idleBT, conditions: () => HasNoMoveInput);
            //
            // //PrimaryAttack00 -> Idle
            // _primaryAttackBT00.AddTransition(to: _idleBT, conditions: () => _primaryAttackBT00.IsDoneAttacking && HasNoMoveInput);
            // //PrimaryAttack01 -> idle
            // _primaryAttackBT01.AddTransition(to: _idleBT, conditions: () => _primaryAttackBT01.IsDoneAttacking && HasNoMoveInput);
            // //PrimaryAttack02 -> idle
            // _primaryAttackBT02.AddTransition(to: _idleBT, conditions: () => _primaryAttackBT02.IsDoneAttacking && HasNoMoveInput);
            //
            // //PrimaryAttack00 -> Walk
            // _primaryAttackBT00.AddTransition(to: _walkBT, conditions: () => _primaryAttackBT00.IsDoneAttacking && HasMoveInput);
            // //PrimaryAttack01 -> walk
            // _primaryAttackBT01.AddTransition(to: _walkBT, conditions: () => _primaryAttackBT01.IsDoneAttacking && HasMoveInput);
            // //PrimaryAttack02 -> walk
            // _primaryAttackBT02.AddTransition(to: _walkBT, conditions: () => _primaryAttackBT02.IsDoneAttacking && HasMoveInput);
            //
            // //Idle -> PrimaryAttack00
            // _idleBT.AddTransition(to: _primaryAttackBT00, conditions: () => playerReferences.InputHandler.PrimaryFireInput);
            // //Walk -> PrimaryAttack00
            // _walkBT.AddTransition(to: _primaryAttackBT00, conditions: () => playerReferences.InputHandler.PrimaryFireInput);
            //
            // //PrimaryAttack00 -> PrimaryAttack01
            // _primaryAttackBT00.AddTransition(to: _primaryAttackBT01, conditions: () => playerReferences.InputHandler.PrimaryFireInput && _primaryAttackBT00.CanGoIntoNextAttack);
            //
            // //PrimaryAttack01 -> PrimaryAttack02
            // _primaryAttackBT01.AddTransition(to: _primaryAttackBT02, conditions: () => playerReferences.InputHandler.PrimaryFireInput && _primaryAttackBT01.CanGoIntoNextAttack);
            //
            // #endregion
        }

        private void OnEnable()  => EnableLateFixedUpdate();
        private void OnDisable() => DisableLateFixedUpdate();

        private void Update()
        {
            _root.Update();
        }

        private void FixedUpdate()
        {
            _root.FixedUpdate();
        }

        private void LateUpdate()
        {
            _root.LateUpdate();
        }

        #region LateFixedUpdate
        
        private IEnumerator LateFixedUpdate()
        {
            while (true)
            {
                yield return _waitForFixedUpdate;

                _root.LateFixedUpdate();
            }
        }

        private readonly WaitForFixedUpdate _waitForFixedUpdate = new();
        private Coroutine _lateFixedUpdateCoroutine;
        private void EnableLateFixedUpdate()
        {
            if (_lateFixedUpdateCoroutine != null)
            {
                StopCoroutine(routine: _lateFixedUpdateCoroutine);
            }
            _lateFixedUpdateCoroutine = StartCoroutine(LateFixedUpdate());
        }
        private void DisableLateFixedUpdate()
        {
            if (_lateFixedUpdateCoroutine != null)
            {
                StopCoroutine(routine: _lateFixedUpdateCoroutine);
            }
        }
        
        #endregion

        #region Commands

        [Command(aliasOverride: "Player.Health.Kill")]
        [UsedImplicitly]
        public void KillPlayer()
        {
            Debug.Log("Killing player");
            playerAttributes.health.Value = 0;
        }
        
        [Command(aliasOverride: "Player.Health.Add")]
        [UsedImplicitly]
        public void AddHealth(UInt16 amount)
        {
            Debug.Log("Adding health " + amount);
            playerAttributes.health.Value += amount;
        }
        
        [Command(aliasOverride: "Player.Health.Sub")]
        [UsedImplicitly]
        public void SubHealth(UInt16 amount)
        {
            Debug.Log("Subtracting health " + amount);
            playerAttributes.health.Value -= amount;
        }

        #endregion
    }
}