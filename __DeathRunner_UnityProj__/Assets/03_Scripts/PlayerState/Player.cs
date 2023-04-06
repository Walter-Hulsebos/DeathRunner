using DeathRunner.Inputs;
using HFSM;
using UnityEngine;

using static Unity.Mathematics.math;

using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

namespace DeathRunner.Shared.StateMachine
{
    public sealed class Player : MonoBehaviour
    {
        //NOTE: [Walter] Make shared states possible??
        
        //[SerializeField] private InputActionReference _inputActionReference;
        [SerializeField] private InputHandler inputHandler;
        
        private State     _root;
        
        private State     _alive;
        private StateLeaf _dead;

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
                        _idleNT            = new PlayerStateLeaf_Idle(), 
                        _walkNT            = new PlayerStateLeaf_Walk(), 
                        _dashNT            = new PlayerStateLeaf_Dash(), 
                        _primaryAttackNT   = new PlayerStateLeaf_Primary(), 
                        _secondaryAttackNT = new PlayerStateLeaf_Secondary()), 
                    _bulletTime = new PlayerState_BulletTime(/*params childstates */
                        _idleBT            = new PlayerStateLeaf_Idle(), 
                        _walkBT            = new PlayerStateLeaf_Walk(), 
                        _primaryAttackBT   = new PlayerStateLeaf_Primary())), 
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
            
            //inputHandler.OnMoveInputUpdated += moveInput => idleNormalTime.AddEventTransition(to: walkNormalTime, conditions: () => any(moveInput != F32x2.zero));
            
            //inputHandler.OnMoveStarted += moveInput => _idleNT.AddEventTransitionTo(  _walkNT);
            //inputHandler.OnMoveStopped += moveInput => _idleNT.AddEventTransitionFrom(_walkNT);
            
            //Idle <-> Walk
            _idleNT.AddTransitionTo(  _walkNT, conditions: () => any(inputHandler.MoveInput != F32x2.zero));
            _idleNT.AddTransitionFrom(_walkNT, conditions: () => all(inputHandler.MoveInput == F32x2.zero));
            
            //Idle <-> Dash
            _idleNT.AddTransitionTo(  _dashNT, conditions: () => inputHandler.DashInput);
            _idleNT.AddTransitionFrom(_dashNT, conditions: () => all(inputHandler.MoveInput == F32x2.zero));
            
            //inputHandler.OnDashTriggered += () => _idleNT.AddEventTransitionTo(_dashNT);
            //_idleNT.AddTransitionFrom(_dashNT);
            
            //inputHandler.OnPrimaryFireStarted += () => _idleNT.AddEventTransitionTo(  _primaryAttackNT);
            //inputHandler.OnPrimaryFireStopped += () => _idleNT.AddEventTransitionFrom(_primaryAttackNT);
            
            //inputHandler.OnSecondaryFireStarted += () => _idleNT.AddEventTransitionTo(  _secondaryAttackNT);
            //inputHandler.OnSecondaryFireStopped += () => _idleNT.AddEventTransitionFrom(_secondaryAttackNT);

            #endregion

            #region Walk Normal Time
            
            //inputHandler.OnDashTriggered += () => _walkNT.AddEventTransitionTo(_dashNT);
            _walkNT.AddTransitionTo(  _dashNT, conditions: () => inputHandler.DashInput);
            _walkNT.AddTransitionFrom(_dashNT, conditions: () => any(inputHandler.MoveInput != F32x2.zero));
            
            inputHandler.OnPrimaryFireStarted += () => _walkNT.AddEventTransitionTo(  _primaryAttackNT);
            inputHandler.OnPrimaryFireStopped += () => _walkNT.AddEventTransitionFrom(_primaryAttackNT);
            
            inputHandler.OnSecondaryFireStarted += () => _walkNT.AddEventTransitionTo(  _secondaryAttackNT);
            inputHandler.OnSecondaryFireStopped += () => _walkNT.AddEventTransitionFrom(_secondaryAttackNT);

            #endregion
            
            #region Primary Normal Time <-> Secondary Normal Time
            
            inputHandler.OnSecondaryFireStarted += () => _primaryAttackNT.AddEventTransitionTo(  _secondaryAttackNT);
            inputHandler.OnSecondaryFireStopped += () => _primaryAttackNT.AddEventTransitionFrom(_secondaryAttackNT);
            
            #endregion
            
            #region Idle Bullet Time
            
            inputHandler.OnMoveStarted += moveInput => _idleBT.AddEventTransitionTo(  _walkBT);
            inputHandler.OnMoveStopped += moveInput => _idleBT.AddEventTransitionFrom(_walkBT);
            
            inputHandler.OnPrimaryFireStarted += () => _idleBT.AddEventTransitionTo(  _primaryAttackBT);
            inputHandler.OnPrimaryFireStopped += () => _idleBT.AddEventTransitionFrom(_primaryAttackBT);

            #endregion
            
            #region Walk Bullet Time
            
            inputHandler.OnPrimaryFireStarted += () => _walkBT.AddEventTransitionTo(  _primaryAttackBT);
            inputHandler.OnPrimaryFireStopped += () => _walkBT.AddEventTransitionFrom(_primaryAttackBT);
            
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

        private void OnEnable()
        {
            //root.OnEnable();
        }

        private void OnDisable()
        {
            //root.OnDisable();
        }

        private void OnDestroy()
        {
            //root.OnDestroy();
        }
    }
}
