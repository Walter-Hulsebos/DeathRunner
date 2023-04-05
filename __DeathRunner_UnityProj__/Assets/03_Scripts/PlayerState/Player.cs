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

        private StateLeaf _idleNormalTime;
        private StateLeaf _walkNormalTime;
        private StateLeaf _dashNormalTime;
        private StateLeaf _primaryAttackNormalTime;
        private StateLeaf _secondaryAttackNormalTime;

        private StateLeaf _idleBulletTime;
        private StateLeaf _walkBulletTime;
        private StateLeaf _primaryAttackBulletTime;

        private void Awake()
        {
            CreateStateTree();
            CreateStateTransitions();
        }

        private void CreateStateTree()
        {
            // root = new PlayerState_Root()
            //     .AddChild(alive                             = new PlayerState_Alive()
            //         .AddChild(normalTime                    = new PlayerState_NormalTime()
            //             .AddChild(idleNormalTime            = new PlayerStateLeaf_Idle())
            //             .AddChild(walkNormalTime            = new PlayerStateLeaf_Walk())
            //             .AddChild(dashNormalTime            = new PlayerStateLeaf_Dash())
            //             .AddChild(primaryAttackNormalTime   = new PlayerStateLeaf_Primary())
            //             .AddChild(secondaryAttackNormalTime = new PlayerStateLeaf_Secondary()))
            //         .AddChild(bulletTime                    = new PlayerState_BulletTime()
            //             .AddChild(idleBulletTime            = new PlayerStateLeaf_Idle())
            //             .AddChild(walkBulletTime            = new PlayerStateLeaf_Walk())
            //             .AddChild(primaryAttackBulletTime   = new PlayerStateLeaf_Primary())))
            //     .AddChild(dead = new PlayerStateLeaf_Dead());
            
            // _root = new PlayerState_Root(/*params childstates */_alive , _dead);
            //     _alive = new PlayerState_Alive(/*params childstates */_normalTime, _bulletTime);
            //         _normalTime = new PlayerState_NormalTime(/*params childstates */_idleNormalTime, _walkNormalTime, _dashNormalTime, _primaryAttackNormalTime, _secondaryAttackNormalTime);
            //             _idleNormalTime            = new PlayerStateLeaf_Idle();
            //             _walkNormalTime            = new PlayerStateLeaf_Walk();
            //             _dashNormalTime            = new PlayerStateLeaf_Dash();
            //             _primaryAttackNormalTime   = new PlayerStateLeaf_Primary();
            //             _secondaryAttackNormalTime = new PlayerStateLeaf_Secondary();
            //         _bulletTime = new PlayerState_BulletTime(/*params childstates */_idleBulletTime, _walkBulletTime, _primaryAttackBulletTime);
            //             _idleBulletTime          = new PlayerStateLeaf_Idle();
            //             _walkBulletTime          = new PlayerStateLeaf_Walk();
            //             _primaryAttackBulletTime = new PlayerStateLeaf_Primary();
            //     _dead = new PlayerStateLeaf_Dead();
            
                    _idleNormalTime            = new PlayerStateLeaf_Idle();
                    _walkNormalTime            = new PlayerStateLeaf_Walk();
                    _dashNormalTime            = new PlayerStateLeaf_Dash();
                    _primaryAttackNormalTime   = new PlayerStateLeaf_Primary();
                    _secondaryAttackNormalTime = new PlayerStateLeaf_Secondary();
                _normalTime = new PlayerState_NormalTime(/*params childstates */_idleNormalTime, _walkNormalTime, _dashNormalTime, _primaryAttackNormalTime, _secondaryAttackNormalTime);
                    _idleBulletTime          = new PlayerStateLeaf_Idle();
                    _walkBulletTime          = new PlayerStateLeaf_Walk();
                    _primaryAttackBulletTime = new PlayerStateLeaf_Primary();
                _bulletTime = new PlayerState_BulletTime(/*params childstates */_idleBulletTime, _walkBulletTime, _primaryAttackBulletTime);
                _alive = new PlayerState_Alive(/*params childstates */_normalTime, _bulletTime);
                _dead = new PlayerStateLeaf_Dead();
            _root = new PlayerState_Root(/*params childstates */_alive , _dead);
            
            // _root = new PlayerState_Root(/*params childstates */
            //     _alive = new PlayerState_Alive(/*params child states */
            //         _normalTime = new PlayerState_NormalTime(/*params childstates */
            //             _idleNormalTime = new PlayerStateLeaf_Idle(), 
            //             _walkNormalTime = new PlayerStateLeaf_Walk(), 
            //             _dashNormalTime = new PlayerStateLeaf_Dash(), 
            //             _primaryAttackNormalTime = new PlayerStateLeaf_Primary(), 
            //             _secondaryAttackNormalTime = new PlayerStateLeaf_Secondary()), 
            //         _bulletTime = new PlayerState_BulletTime(/*params childstates */
            //             _idleBulletTime = new PlayerStateLeaf_Idle(), 
            //             _walkBulletTime = new PlayerStateLeaf_Walk(), 
            //             _primaryAttackBulletTime = new PlayerStateLeaf_Primary())), 
            //     _dead = new PlayerStateLeaf_Dead());
        }

        private void CreateStateTransitions()
        {
            #region Alive <-> Dead
            
            _alive.AddTransitionTo(  _dead);
            _alive.AddTransitionFrom(_dead);
            
            //_alive.AddAnyTransition(_dead);

            #endregion
            
            #region Normal Time <-> Bullet Time

            _normalTime.AddTransitionTo(  _bulletTime);
            _bulletTime.AddTransitionFrom(_normalTime);
            
            //_normalTime.AddAnyTransition(_bulletTime);
            //_bulletTime.AddAnyTransition(_normalTime);
            
            #endregion

            #region Idle Normal Time
            
            //inputHandler.OnMoveInputUpdated += moveInput => idleNormalTime.AddEventTransition(to: walkNormalTime, conditions: () => all(moveInput != F32x2.zero));
            
            //_idleNormalTime.AddTransitionTo(  _walkNormalTime);
            //_idleNormalTime.AddTransitionFrom(_walkNormalTime);
            inputHandler.OnMoveStarted += moveInput => _idleNormalTime.AddEventTransition(to: _walkNormalTime);
            inputHandler.OnMoveStopped += moveInput => _walkNormalTime.AddEventTransition(to: _idleNormalTime);

            //_idleNormalTime.AddTransitionTo(  _dashNormalTime);
            //_idleNormalTime.AddTransitionFrom(_dashNormalTime);
            inputHandler.OnDashTriggered += () => _idleNormalTime.AddEventTransition(to: _dashNormalTime);

            //_idleNormalTime.AddTransitionTo(  _primaryAttackNormalTime);
            //_idleNormalTime.AddTransitionFrom(_primaryAttackNormalTime);
            inputHandler.OnPrimaryFireStarted += () => _idleNormalTime.AddEventTransition(to: _primaryAttackNormalTime);
            inputHandler.OnPrimaryFireStopped += () => _primaryAttackNormalTime.AddEventTransition(to: _idleNormalTime);

            //_idleNormalTime.AddTransitionTo(  _secondaryAttackNormalTime);
            //_idleNormalTime.AddTransitionFrom(_secondaryAttackNormalTime);
            inputHandler.OnSecondaryFireStarted += () => _idleNormalTime.AddEventTransition(to: _secondaryAttackNormalTime);
            inputHandler.OnSecondaryFireStopped += () => _secondaryAttackNormalTime.AddEventTransition(to: _idleNormalTime);

            #endregion

            #region Walk Normal Time

            //_walkNormalTime.AddTransitionTo(  _idleNormalTime);
            //_walkNormalTime.AddTransitionFrom(_idleNormalTime);
            inputHandler.OnMoveStarted += moveInput => _walkNormalTime.AddEventTransition(to: _idleNormalTime);
            inputHandler.OnMoveStopped += moveInput => _idleNormalTime.AddEventTransition(to: _walkNormalTime);

            //_walkNormalTime.AddTransitionTo(  _dashNormalTime);
            //_walkNormalTime.AddTransitionFrom(_dashNormalTime);
            inputHandler.OnDashTriggered += () => _walkNormalTime.AddEventTransition(to: _dashNormalTime);

            //_walkNormalTime.AddTransitionTo(  _primaryAttackNormalTime);
            //_walkNormalTime.AddTransitionFrom(_primaryAttackNormalTime);
            inputHandler.OnPrimaryFireStarted += () => _walkNormalTime.AddEventTransition(to: _primaryAttackNormalTime);
            inputHandler.OnPrimaryFireStopped += () => _primaryAttackNormalTime.AddEventTransition(to: _walkNormalTime);
            
            //_walkNormalTime.AddTransitionTo(  _secondaryAttackNormalTime);
            //_walkNormalTime.AddTransitionFrom(_secondaryAttackNormalTime);
            inputHandler.OnSecondaryFireStarted += () => _walkNormalTime.AddEventTransition(to: _secondaryAttackNormalTime);
            inputHandler.OnSecondaryFireStopped += () => _secondaryAttackNormalTime.AddEventTransition(to: _walkNormalTime);

            #endregion
            
            #region Primary Normal Time <-> Secondary Normal Time
            
            //_primaryAttackNormalTime.AddTransitionTo(  _secondaryAttackNormalTime);
            //_primaryAttackNormalTime.AddTransitionFrom(_secondaryAttackNormalTime);
            inputHandler.OnSecondaryFireStarted += () => _primaryAttackNormalTime.AddEventTransition(to: _secondaryAttackNormalTime);
            inputHandler.OnSecondaryFireStopped += () => _secondaryAttackNormalTime.AddEventTransition(to: _primaryAttackNormalTime);
            
            #endregion
            
            #region Idle Bullet Time
            
            //_idleBulletTime.AddTransitionTo(  _walkBulletTime);
            //_idleBulletTime.AddTransitionFrom(_walkBulletTime);
            inputHandler.OnMoveStarted += moveInput => _idleBulletTime.AddEventTransition(to: _walkBulletTime);
            inputHandler.OnMoveStopped += moveInput => _walkBulletTime.AddEventTransition(to: _idleBulletTime);

            //_idleBulletTime.AddTransitionTo(  _primaryAttackBulletTime);
            //_idleBulletTime.AddTransitionFrom(_primaryAttackBulletTime);
            inputHandler.OnPrimaryFireStarted += () => _idleBulletTime.AddEventTransition(to: _primaryAttackBulletTime);
            inputHandler.OnPrimaryFireStopped += () => _primaryAttackBulletTime.AddEventTransition(to: _idleBulletTime);

            #endregion
            
            #region Walk Bullet Time
            
            //_walkBulletTime.AddTransitionTo(  _idleBulletTime);
            //_walkBulletTime.AddTransitionFrom(_idleBulletTime);
            inputHandler.OnMoveStarted += moveInput => _walkBulletTime.AddEventTransition(to: _idleBulletTime);
            inputHandler.OnMoveStopped += moveInput => _idleBulletTime.AddEventTransition(to: _walkBulletTime);
            
            //_walkBulletTime.AddTransitionTo(  _primaryAttackBulletTime);
            //_walkBulletTime.AddTransitionFrom(_primaryAttackBulletTime);
            inputHandler.OnPrimaryFireStarted += () => _walkBulletTime.AddEventTransition(to: _primaryAttackBulletTime);
            inputHandler.OnPrimaryFireStopped += () => _primaryAttackBulletTime.AddEventTransition(to: _walkBulletTime);
            
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
