//System libraries first
using System;
using System.Collections;

//Unity-specific libraries next
using UnityEngine;
using static Unity.Mathematics.math;

//Third-party libraries next
using HFSM;
using JetBrains.Annotations;
using QFSW.QC;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;

using U16   = System.UInt16; //max 65,535

namespace DeathRunner.Player
{
    public sealed class Player : MonoBehaviour
    {
        //NOTE: [Walter] Make shared states possible??

        [SerializeField] private PlayerReferences   playerReferences = new();

        [Tooltip("Locomotion Settings")]
        [SerializeField] private LocomotionSettings locomotionNTSettings;
        
        [Tooltip("Idle Settings")]
        [SerializeField] private IdleSettings       idleNTSettings;
        [Tooltip("Walk Settings")]
        [SerializeField] private MoveSettings       moveNtSettings;

        [Tooltip("Long Dash Settings")]
        [SerializeField] private DashLongSettings   dashLongNTSettings;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Combat")]
        #endif
        [SerializeField] private MeleeAttackData primaryAttack00NT;
        #if ODIN_INSPECTOR
        [FoldoutGroup("Combat")]
        #endif
        [SerializeField] private MeleeAttackData primaryAttack01NT;
        #if ODIN_INSPECTOR
        [FoldoutGroup("Combat")]
        #endif
        [SerializeField] private MeleeAttackData primaryAttack02NT;

        private State                           _root;
        
        private PlayerState_Alive               _alive;
        private PlayerStateLeaf_Dead            _dead;

        [UsedImplicitly]
        private PlayerState_Locomotion          _locomotionNT;
        
        private PlayerStateLeaf_Idle            _idleNT;
        private PlayerStateLeaf_Move            _walkNT;
        
        private PlayerStateLeaf_DashLong        _dashLongNT;
        
        private PlayerStateLeaf_AttackMelee     _lightAttackNt00;
        private PlayerStateLeaf_AttackMelee     _lightAttackNt01;
        private PlayerStateLeaf_AttackMelee     _lightAttackNt02;
        
        //private PlayerStateLeaf_SecondaryAttack _secondaryAttackNT;


        #if UNITY_EDITOR
        private void Reset() => playerReferences.Reset(gameObject);

        private void OnValidate() => playerReferences.OnValidate(gameObject);
        
        #if ODIN_INSPECTOR
        [Button]
        #endif
        [ContextMenu("Ensure References")]
        private void EnsureReferences() => playerReferences.OnValidate(gameObject);
        #endif
        
        private Boolean HasMoveInput   => any(playerReferences.InputHandler.MoveInput != F32x2.zero);
        private Boolean HasNoMoveInput => !HasMoveInput;
        
        private Boolean DashInputIsHeld     => playerReferences.InputHandler.DashInputIsHeld;
        private Boolean DashInputIsNotHeld  => !DashInputIsHeld;
        
        //private Boolean HasStaminaLeft   => playerReferences.Stamina.stamina.Value > 0;
        //private Boolean HasNoStaminaLeft => !HasStaminaLeft;

        private void Awake()
        {
            playerReferences.Init(gameObject);
            
            CreateStateTree();
            CreateStateTransitions();

            //Init root state
            _root.Init();
        }

        private void CreateStateTree()
        {
            _root = new PlayerState_Root
            (/*params child states */
                _alive = new PlayerState_Alive
                (/*params child states */
                    _locomotionNT = new PlayerState_Locomotion
                    (settings: locomotionNTSettings, references: playerReferences, /*params child states */
                        _idleNT            = new PlayerStateLeaf_Idle(settings: idleNTSettings, references: playerReferences), 
                        _walkNT            = new PlayerStateLeaf_Move(settings: moveNtSettings, references: playerReferences)
                    ),
                    _dashLongNT      = new PlayerStateLeaf_DashLong( settings: dashLongNTSettings,  references: playerReferences),
                    
                    _lightAttackNt00 = new PlayerStateLeaf_AttackMelee(settings: primaryAttack00NT.Settings, references: playerReferences),
                    _lightAttackNt01 = new PlayerStateLeaf_AttackMelee(settings: primaryAttack01NT.Settings, references: playerReferences),
                    _lightAttackNt02 = new PlayerStateLeaf_AttackMelee(settings: primaryAttack02NT.Settings, references: playerReferences)//,
                    
                    //_secondaryAttackNT = new PlayerStateLeaf_SecondaryAttack()
                ),
                _dead = new PlayerStateLeaf_Dead()
            );
        }
        
        //TODO: [Walter] InputBuffer/Queue for attacks, if you click the attack button within the timeslot of the previous attack, it will queue the next attack, and execute it after the previous one is finished.
        //Currently, if you click the attack button while the previous attack is still executing, it will cancel the previous attack and execute the new one.

        private void CreateStateTransitions()
        {
            _alive.AddTransition(to: _dead, conditions: () => playerReferences.Health.health.IsZero == true);  //Alive -> Dead
            _dead.AddTransition(to: _alive, conditions: () => playerReferences.Health.health.IsZero == false); //Dead -> Alive

            _idleNT.AddTransition(to: _walkNT, conditions: () => HasMoveInput);   //Idle -> Walk
            _walkNT.AddTransition(to: _idleNT, conditions: () => HasNoMoveInput); //Walk -> Idle
            
            // _idleNT.AddTransition(to: _dashShortNT, conditions: () => playerReferences.InputHandler.shortDashInputQueue.Peek, 
            //     transitionAction: () => playerReferences.InputHandler.shortDashInputQueue.Dequeue()); //Idle -> DashShort
            // _dashShortNT.AddTransition(to: _idleNT, conditions: () => _dashShortNT.IsDoneDashing && HasNoMoveInput);                //DashShort -> Idle
            
            // _walkNT.AddTransition(to: _dashShortNT, conditions: () => playerReferences.InputHandler.shortDashInputQueue.Peek, 
            //     transitionAction: () => playerReferences.InputHandler.shortDashInputQueue.Dequeue()); //Walk -> DashShort
            // _dashShortNT.AddTransition(to: _walkNT, conditions: () => _dashShortNT.IsDoneDashing && HasMoveInput);                  //DashShort -> Walk
            
            _idleNT.AddTransition(to: _dashLongNT, conditions: () => DashInputIsHeld); //Idle -> DashLong
            _dashLongNT.AddTransition(to: _idleNT, conditions: () => DashInputIsNotHeld && HasNoMoveInput); //DashLong -> Idle
            //_dashLongNT.AddTransition(to: _idleNT, conditions: () => HasNoStaminaLeft   && HasNoMoveInput); //DashLong -> Idle
            
            _walkNT.AddTransition(to: _dashLongNT, conditions: () => DashInputIsHeld); //Walk -> DashLong
            _dashLongNT.AddTransition(to: _walkNT, conditions: () => DashInputIsNotHeld && HasMoveInput); //DashLong -> Walk
            //_dashLongNT.AddTransition(to: _walkNT, conditions: () => HasNoStaminaLeft   && HasMoveInput); //DashLong -> Walk
            
            //TODO: Add post transitions after attacks when back to walk/idle in which you can still follow up with another attack.
            
            _lightAttackNt00.AddTransition(to: _idleNT, conditions: () => _lightAttackNt00.CanFadeOut && HasNoMoveInput);  //lightAttackNt00 -> idle
            _lightAttackNt01.AddTransition(to: _idleNT, conditions: () => _lightAttackNt01.CanFadeOut && HasNoMoveInput);  //lightAttackNt00 -> idle
            _lightAttackNt02.AddTransition(to: _idleNT, conditions: () => _lightAttackNt02.CanFadeOut && HasNoMoveInput);  //lightAttackNt00 -> idle
            _idleNT.AddTransition(to: _lightAttackNt00, conditions: () => playerReferences.InputHandler.primaryFireInputQueue.Peek, 
                transitionAction: () => playerReferences.InputHandler.primaryFireInputQueue.Dequeue()); //idle -> lightAttackNt00
            
            _lightAttackNt00.AddTransition(to: _walkNT, conditions: () => _lightAttackNt00.CanFadeOut && HasMoveInput);    //lightAttackNt00 -> walk
            _lightAttackNt01.AddTransition(to: _walkNT, conditions: () => _lightAttackNt01.CanFadeOut && HasMoveInput);    //lightAttackNt00 -> walk
            _lightAttackNt02.AddTransition(to: _walkNT, conditions: () => _lightAttackNt02.CanFadeOut && HasMoveInput);    //lightAttackNt00 -> walk
            _walkNT.AddTransition(to: _lightAttackNt00, conditions: () => playerReferences.InputHandler.primaryFireInputQueue.Peek, 
                transitionAction: () => playerReferences.InputHandler.primaryFireInputQueue.Dequeue()); //walk -> lightAttackNt00
            
            _lightAttackNt00.AddTransition(to: _lightAttackNt01, conditions: () => playerReferences.InputHandler.primaryFireInputQueue.Peek && _lightAttackNt00.CanGoIntoNextAttack,  
                transitionAction: () => playerReferences.InputHandler.primaryFireInputQueue.Dequeue()); //lightAttackNt00 -> lightAttackNt01
            _lightAttackNt01.AddTransition(to: _lightAttackNt02, conditions: () => playerReferences.InputHandler.primaryFireInputQueue.Peek && _lightAttackNt01.CanGoIntoNextAttack,  
                transitionAction: () => playerReferences.InputHandler.primaryFireInputQueue.Dequeue() ); //lightAttackNt01 -> lightAttackNt02
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

        [Command(aliasOverride: "Player.Health")]
        [UsedImplicitly]
        public UInt16 Health
        {
            get
            {
                #if UNITY_EDITOR
                Debug.Log("Getting health", context: this);
                #endif
                return playerReferences.Health.health.Value;
            }
            set
            {
                #if UNITY_EDITOR
                Debug.Log("Setting health to " + value, context: this);
                #endif
                playerReferences.Health.health.Value = value;
            }
        }

        [Command(aliasOverride: "Player.Health.Kill")]
        [UsedImplicitly]
        public void KillPlayer()
        {
            #if UNITY_EDITOR
            Debug.Log("Killing player", context: this);
            #endif
            playerReferences.Health.health.Value = 0;
        }
        
        [Command(aliasOverride: "Player.Health.Add")]
        [UsedImplicitly]
        public void AddHealth(U16 amount)
        {
            #if UNITY_EDITOR
            Debug.Log("Adding health " + amount, context: this);
            #endif
            playerReferences.Health.health.Value += amount;
        }
        
        [Command(aliasOverride: "Player.Health.Sub")]
        [UsedImplicitly]
        public void SubHealth(U16 amount)
        {
            #if UNITY_EDITOR
            Debug.Log("Subtracting health " + amount, context: this);
            #endif
            playerReferences.Health.health.Value -= amount;
        }

        #endregion
    }
}