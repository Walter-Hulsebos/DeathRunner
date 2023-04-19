//System libraries first

//Unity-specific libraries next

using System;
using System.Collections;
using HFSM;
using UnityEngine;
using UnityEngine.Serialization;
using static Unity.Mathematics.math;

//Third-party libraries next

//Project-specific libraries last
using F32x2 = Unity.Mathematics.float2;

namespace DeathRunner.PlayerState
{
    public sealed class Player : MonoBehaviour
    {
        //NOTE: [Walter] Make shared states possible??

        [SerializeField] private PlayerReferences playerReferences = new();

        [Tooltip("Locomotion Settings for Normal-Time")]
        [SerializeField] private LocomotionSettings locomotionNTSettings;
        
        [Tooltip("Idle Settings for Normal-Time")]
        [SerializeField] private IdleSettings idleNTSettings;
        [FormerlySerializedAs("walkNTSettings")]
        [Tooltip("Walk Settings for Normal-Time")]
        [SerializeField] private MoveSettings moveNtSettings;
        
        [Tooltip("Dash Settings for Normal-Time")]
        [SerializeField] private DashSettings dashNTSettings;
        
        [FormerlySerializedAs("primaryAttack01NTSettings")] [SerializeField] private PrimaryAttackSettings primaryAttack00NTSettings;
        [FormerlySerializedAs("primaryAttack02NTSettings")] [SerializeField] private PrimaryAttackSettings primaryAttack01NTSettings;
        [FormerlySerializedAs("primaryAttack03NTSettings")] [SerializeField] private PrimaryAttackSettings primaryAttack02NTSettings;
        
        //[SerializeField] private SecondaryAttackSettings secondaryAttack01NTSettings;
        
        
        //[SerializeField] private AttackSettings
        [Tooltip("Locomotion Settings for Bullet-Time")]
        [SerializeField] private LocomotionSettings locomotionBTSettings;
        
        [Tooltip("Idle Settings for Bullet-Time")]
        [SerializeField] private IdleSettings idleBTSettings;
        [FormerlySerializedAs("walkBTSettings")]
        [Tooltip("Walk Settings for Bullet-Time")]
        [SerializeField] private MoveSettings moveBtSettings;

        private State     _root;
        
        private State     _alive;
        private StateLeaf _dead;

        private State     _normalTime;
        private State     _bulletTime;
        
        // Suffixed with "NT" (NormalTime) or "BT" (BulletTime) to avoid name conflicts
        private State     _locomotionNT;
        
        private StateLeaf _idleNT;
        private StateLeaf _walkNT;
        
        private PlayerStateLeaf_Dash            _dashNT;
        private PlayerStateLeaf_PrimaryAttack   _primaryAttackNT00;
        private PlayerStateLeaf_PrimaryAttack   _primaryAttackNT01;
        private PlayerStateLeaf_PrimaryAttack   _primaryAttackNT02;
        
        private PlayerStateLeaf_SecondaryAttack _secondaryAttackNT;
        
        private State     _locomotionBT;

        private StateLeaf _idleBT;
        private StateLeaf _walkBT;
        
        private StateLeaf _primaryAttackBT;

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
                        _dashNT            = new PlayerStateLeaf_Dash(settings: dashNTSettings, references: playerReferences), 
                        _primaryAttackNT00 = new PlayerStateLeaf_PrimaryAttack(settings: primaryAttack00NTSettings, references: playerReferences),
                        _primaryAttackNT01 = new PlayerStateLeaf_PrimaryAttack(settings: primaryAttack01NTSettings, references: playerReferences),
                        _primaryAttackNT02 = new PlayerStateLeaf_PrimaryAttack(settings: primaryAttack02NTSettings, references: playerReferences),
                        
                        _secondaryAttackNT = new PlayerStateLeaf_SecondaryAttack()
                    ), 
                    _bulletTime = new PlayerState_BulletTime
                    (/*params child states */
                        _locomotionBT = new PlayerState_Locomotion
                        (settings: locomotionBTSettings, references: playerReferences, /*params child states */
                            _idleBT            = new PlayerStateLeaf_Idle(settings: idleBTSettings, references: playerReferences),
                            _walkBT            = new PlayerStateLeaf_Move(settings: moveBtSettings, references: playerReferences)
                        )
                        //_primaryAttackBT   = new PlayerStateLeaf_PrimaryAttack(settings: primaryAttack00NTSettings, references: playerReferences))
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
            
            //Normal Time <-> Bullet Time

            //IdleNT -> WalkNT
            _idleNT.AddTransition(to: _walkNT, conditions: () => HasMoveInput);
            //WalkNT -> IdleNT
            _walkNT.AddTransition(to: _idleNT, conditions: () => HasNoMoveInput);
            
            //IdleNT -> DashNT
            _idleNT.AddTransition(to: _dashNT, conditions: () => HasDashInput);
            //DashNT -> IdleNT
            _dashNT.AddTransition(to: _idleNT, conditions: () => HasNoMoveInput && _dashNT.IsDoneDashing);

            //WalkNT -> DashNT
            _walkNT.AddTransition(to: _dashNT, conditions: () => HasDashInput);
            //DashNT -> WalkNT
            _dashNT.AddTransition(to: _walkNT, conditions: () => HasMoveInput && _dashNT.IsDoneDashing);

            //Idle -> Walk
            _idleBT.AddTransition(to: _walkBT, conditions: () => HasMoveInput);
            //Walk -> Idle
            _walkBT.AddTransition(to: _idleBT, conditions: () => HasNoMoveInput);

            //Idle -> PrimaryAttack00
            _idleNT.AddTransition(to: _primaryAttackNT00, conditions: () => playerReferences.InputHandler.PrimaryFireInput);
            
            //PrimaryAttack00 -> Idle
            _primaryAttackNT00.AddTransition(to: _idleNT, conditions: () => _primaryAttackNT00.IsDoneAttacking && HasNoMoveInput);
            //PrimaryAttack01 -> idle
            _primaryAttackNT01.AddTransition(to: _idleNT, conditions: () => _primaryAttackNT01.IsDoneAttacking && HasNoMoveInput);
            //PrimaryAttack02 -> idle
            _primaryAttackNT02.AddTransition(to: _idleNT, conditions: () => _primaryAttackNT02.IsDoneAttacking && HasNoMoveInput);

            //Walk -> PrimaryAttack00
            _walkNT.AddTransition(to: _primaryAttackNT00, conditions: () => playerReferences.InputHandler.PrimaryFireInput);
            
            //PrimaryAttack00 -> Walk
            _primaryAttackNT00.AddTransition(to: _walkNT, conditions: () => _primaryAttackNT00.IsDoneAttacking && HasMoveInput);
            //PrimaryAttack01 <- walk
            _primaryAttackNT01.AddTransition(to: _walkNT, conditions: () => _primaryAttackNT01.IsDoneAttacking && HasMoveInput);
            //PrimaryAttack02 <- walk
            _primaryAttackNT02.AddTransition(to: _walkNT, conditions: () => _primaryAttackNT02.IsDoneAttacking && HasMoveInput);
            
            //PrimaryAttack00 -> PrimaryAttack01
            _primaryAttackNT00.AddTransition(to: _primaryAttackNT01, conditions: () => playerReferences.InputHandler.PrimaryFireInput && _primaryAttackNT00.CanGoIntoNextAttack);
            
            //PrimaryAttack01 -> PrimaryAttack02
            _primaryAttackNT01.AddTransition(to: _primaryAttackNT02, conditions: () => playerReferences.InputHandler.PrimaryFireInput && _primaryAttackNT01.CanGoIntoNextAttack);
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
    }
}
