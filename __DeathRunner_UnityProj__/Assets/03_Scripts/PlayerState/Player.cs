//System libraries first

//Unity-specific libraries next
using System.Collections;
using HFSM;
using UnityEngine;
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

        [Tooltip("Idle Settings for Normal-Time")]
        [SerializeField] private IdleSettings idleNTSettings;
        [Tooltip("Walk Settings for Normal-Time")]
        [SerializeField] private WalkSettings walkNTSettings;
        [Tooltip("Dash Settings for Normal-Time")]
        [SerializeField] private DashSettings dashNTSettings;
        
        //[SerializeField] private AttackSettings

        [Tooltip("Idle Settings for Bullet-Time")]
        [SerializeField] private IdleSettings idleBTSettings;
        [Tooltip("Walk Settings for Bullet-Time")]
        [SerializeField] private WalkSettings walkBTSettings;

        private State     _root;
        
        private State     _alive;
        [SerializeField] private StateLeaf _dead;

        private State     _normalTime;
        private State     _bulletTime;

        // Suffixed with "NT" (NormalTime) or "BT" (BulletTime) to avoid name conflicts
        private StateLeaf _idleNT;
        private StateLeaf _walkNT;
        private StateLeaf _dashNT;
        private StateLeaf _primaryAttackNT;
        private StateLeaf _secondaryAttackNT;

        private StateLeaf _idleBT;
        private StateLeaf _walkBT;
        private StateLeaf _primaryAttackBT;

        #if UNITY_EDITOR
        private void Reset() => playerReferences.Reset(gameObject);

        private void OnValidate() => playerReferences.OnValidate(gameObject);
        #endif
        
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
                        _idleNT            = new PlayerStateLeaf_Idle(settings: idleNTSettings, references: playerReferences), 
                        _walkNT            = new PlayerStateLeaf_Walk(settings: walkNTSettings, references: playerReferences), 
                        _dashNT            = new PlayerStateLeaf_Dash(), 
                        _primaryAttackNT   = new PlayerStateLeaf_PrimaryAttack(), 
                        _secondaryAttackNT = new PlayerStateLeaf_SecondaryAttack()), 
                    _bulletTime = new PlayerState_BulletTime(/*params childstates */
                        _idleBT            = new PlayerStateLeaf_Idle(settings: idleBTSettings, references: playerReferences),
                        _walkBT            = new PlayerStateLeaf_Walk(settings: walkBTSettings, references: playerReferences), 
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

            #region Idle Normal Time

            //IdleNT <-> WalkNT
            _idleNT.AddTransitionTo(  _walkNT, conditions: () => any(playerReferences.InputHandler.MoveInput != F32x2.zero));
            _idleNT.AddTransitionFrom(_walkNT, conditions: () => all(playerReferences.InputHandler.MoveInput == F32x2.zero));
            
            //IdleNT <-> DashNT
            _idleNT.AddTransitionTo(  _dashNT, conditions: () => playerReferences.InputHandler.DashInput);
            _idleNT.AddTransitionFrom(_dashNT, conditions: () => all(playerReferences.InputHandler.MoveInput == F32x2.zero));

            #endregion

            #region Walk Normal Time
            
            //WalkNT <-> DashNT
            _walkNT.AddTransitionTo(  _dashNT, conditions: () => playerReferences.InputHandler.DashInput);
            _walkNT.AddTransitionFrom(_dashNT, conditions: () => any(playerReferences.InputHandler.MoveInput != F32x2.zero));
            
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

        private void OnEnable()  => EnableLateFixedUpdate();
        private void OnDisable() => DisableLateFixedUpdate();

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

        private readonly WaitForFixedUpdate _waitForFixedUpdate = new();
        private IEnumerator LateFixedUpdate()
        {
            while (true)
            {
                yield return _waitForFixedUpdate;

                _root.LateFixedUpdate();
            }
        }

        #endregion

        private void OnDestroy()
        {
            //root.OnDestroy();
        }
    }
}
