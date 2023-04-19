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
        
        private PlayerStateLeaf_Dash _dashNT;
        private StateLeaf            _primaryAttackNT;
        private StateLeaf            _secondaryAttackNT;
        
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
        }

        private void CreateStateTree()
        {
            _root = new PlayerState_Root(/*params childstates */
                _alive = new PlayerState_Alive(/*params child states */
                    _normalTime = new PlayerState_NormalTime(/*params childstates */
                        _locomotionNT = new PlayerState_Locomotion(settings: locomotionNTSettings, references: playerReferences, /*params childstates */
                            _idleNT            = new PlayerStateLeaf_Idle(settings: idleNTSettings, references: playerReferences), 
                            _walkNT            = new PlayerStateLeaf_Move(settings: moveNtSettings, references: playerReferences)), 
                        _dashNT            = new PlayerStateLeaf_Dash(settings: dashNTSettings, references: playerReferences), 
                        _primaryAttackNT   = new PlayerStateLeaf_PrimaryAttack(), 
                        _secondaryAttackNT = new PlayerStateLeaf_SecondaryAttack()), 
                    _bulletTime = new PlayerState_BulletTime(/*params childstates */
                        _locomotionBT = new PlayerState_Locomotion(settings: locomotionBTSettings, references: playerReferences, /*params childstates */
                            _idleBT            = new PlayerStateLeaf_Idle(settings: idleBTSettings, references: playerReferences),
                            _walkBT            = new PlayerStateLeaf_Move(settings: moveBtSettings, references: playerReferences)),
                        _primaryAttackBT   = new PlayerStateLeaf_PrimaryAttack())), 
                _dead = new PlayerStateLeaf_Dead());
        }

        private void CreateStateTransitions()
        {
            #region Alive <-> Dead
            
            //_alive.AddTransitionTo(  _dead);
            //_alive.AddTransitionFrom(_dead);
            
            //_alive.AddAnyTransition(_dead);

            #endregion
            
            #region Normal Time <-> Bullet Time

            //_normalTime.AddTransitionTo(  _bulletTime);
            //_normalTime.AddTransitionFrom(_bulletTime);

            #endregion
            
            #region Locomotion Normal Time
            
            //LocomotionNT <-> DashNT
            //_locomotionNT.AddTransitionTo(_dashNT, conditions: () => playerReferences.InputHandler.DashInput);

            #endregion

            #region Idle Normal Time

            //IdleNT <-> WalkNT
            _idleNT.AddTransitionTo(  _walkNT, conditions: () => HasMoveInput);
            _idleNT.AddTransitionFrom(_walkNT, conditions: () => HasNoMoveInput);
            
            //IdleNT <-> DashNT
            _idleNT.AddTransitionTo(  _dashNT, conditions: () => playerReferences.InputHandler.DashInput);
            _idleNT.AddTransitionFrom(_dashNT, () => HasNoMoveInput, () => !_dashNT.IsDashing);

            #endregion

            #region Walk Normal Time
            
            //WalkNT <-> DashNT
            _walkNT.AddTransitionTo(  _dashNT, conditions: () => playerReferences.InputHandler.DashInput);
            _walkNT.AddTransitionFrom(_dashNT, () => HasMoveInput, () => !_dashNT.IsDashing);
            
            //WalkNT <-> PrimaryAttackNT
            //playerReferences.InputHandler.OnPrimaryFireStarted += () => _walkNT.AddEventTransitionTo(  _primaryAttackNT);
            //playerReferences.InputHandler.OnPrimaryFireStopped += () => _walkNT.AddEventTransitionFrom(_primaryAttackNT);
            
            //WalkNT <-> SecondaryAttackNT
            //playerReferences.InputHandler.OnSecondaryFireStarted += () => _walkNT.AddEventTransitionTo(  _secondaryAttackNT);
            //playerReferences.InputHandler.OnSecondaryFireStopped += () => _walkNT.AddEventTransitionFrom(_secondaryAttackNT);

            #endregion
            
            #region Primary Normal Time <-> Secondary Normal Time
            
            //playerReferences.InputHandler.OnSecondaryFireStarted += () => _primaryAttackNT.AddEventTransitionTo(  _secondaryAttackNT);
            //playerReferences.InputHandler.OnSecondaryFireStopped += () => _primaryAttackNT.AddEventTransitionFrom(_secondaryAttackNT);
            
            #endregion
            
            #region Idle Bullet Time
            
            //inputHandler.OnMoveStarted += moveInput => _idleBT.AddEventTransitionTo(  _walkBT);
            //inputHandler.OnMoveStopped += moveInput => _idleBT.AddEventTransitionFrom(_walkBT);
            
            //Idle <-> Walk
            _idleBT.AddTransitionTo(  _walkBT, conditions: () => any(playerReferences.InputHandler.MoveInput != F32x2.zero));
            _idleBT.AddTransitionFrom(_walkBT, conditions: () => all(playerReferences.InputHandler.MoveInput == F32x2.zero));
            
            //inputHandler.OnPrimaryFireStarted += () => _idleBT.AddEventTransitionTo(  _primaryAttackBT);
            //inputHandler.OnPrimaryFireStopped += () => _idleBT.AddEventTransitionFrom(_primaryAttackBT);

            #endregion
            
            #region Walk Bullet Time
            
            //Walk <-> PrimaryAttack
            //playerReferences.InputHandler.OnPrimaryFireStarted += () => _walkBT.AddEventTransitionTo(  _primaryAttackBT);
            //playerReferences.InputHandler.OnPrimaryFireStopped += () => _walkBT.AddEventTransitionFrom(_primaryAttackBT);
            
            #endregion
            
            //Initialize root state
            _root.Init();
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
