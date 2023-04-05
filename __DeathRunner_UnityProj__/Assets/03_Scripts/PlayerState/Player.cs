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
        
        private State     root;
        
        private State     alive;
        private StateLeaf dead;

        private State     normalTime;
        private State     bulletTime;

        private StateLeaf idleNormalTime;
        private StateLeaf walkNormalTime;
        private StateLeaf dashNormalTime;
        private StateLeaf primaryAttackNormalTime;
        private StateLeaf secondaryAttackNormalTime;

        private StateLeaf idleBulletTime;
        private StateLeaf walkBulletTime;
        private StateLeaf primaryAttackBulletTime;

        private void Awake()
        {
            CreateStateTree();
            CreateStateTransitions();
        }

        private void CreateStateTree()
        {
            root = new PlayerState_Root(/*params childstates */alive, dead);
                alive = new PlayerState_Alive(/*params childstates */normalTime, bulletTime);
                    normalTime = new PlayerState_NormalTime(/*params childstates */idleNormalTime, walkNormalTime, dashNormalTime, primaryAttackNormalTime, secondaryAttackNormalTime);
                        idleNormalTime            = new PlayerStateLeaf_Idle();
                        walkNormalTime            = new PlayerStateLeaf_Walk();
                        dashNormalTime            = new PlayerStateLeaf_Dash();
                        primaryAttackNormalTime   = new PlayerStateLeaf_Primary();
                        secondaryAttackNormalTime = new PlayerStateLeaf_Secondary();
                    bulletTime = new PlayerState_BulletTime(/*params childstates */idleBulletTime, walkBulletTime, primaryAttackBulletTime);
                        idleBulletTime          = new PlayerStateLeaf_Idle();
                        walkBulletTime          = new PlayerStateLeaf_Walk();
                        primaryAttackBulletTime = new PlayerStateLeaf_Primary();
                dead = new PlayerStateLeaf_Dead();
        }

        private void CreateStateTransitions()
        {
            #region Alive <-> Dead
            
            alive.AddTransitionTo(  dead);
            alive.AddTransitionFrom(dead);
            
            alive.AddAnyTransition(dead);

            #endregion
            
            #region Normal Time <-> Bullet Time

            normalTime.AddTransitionTo(  bulletTime);
            bulletTime.AddTransitionFrom(normalTime);
            
            normalTime.AddAnyTransition(bulletTime);
            bulletTime.AddAnyTransition(normalTime);
            
            #endregion

            #region Idle Normal Time
            
            //inputHandler.OnMoveInputUpdated += moveInput => idleNormalTime.AddEventTransition(to: walkNormalTime, conditions: () => all(moveInput != F32x2.zero));
            
            idleNormalTime.AddTransitionTo(  walkNormalTime);
            idleNormalTime.AddTransitionFrom(walkNormalTime);
            inputHandler.OnMoveStarted += moveInput => idleNormalTime.AddEventTransition(to: walkNormalTime);
            inputHandler.OnMoveStopped += moveInput => walkNormalTime.AddEventTransition(to: idleNormalTime);

            idleNormalTime.AddTransitionTo(  dashNormalTime);
            idleNormalTime.AddTransitionFrom(dashNormalTime);
            inputHandler.OnDashTriggered += () => idleNormalTime.AddEventTransition(to: dashNormalTime);
            //inputHandler.OnDashStopped += () => dashNormalTime.AddEventTransition(to: idleNormalTime);

            idleNormalTime.AddTransitionTo(  primaryAttackNormalTime);
            idleNormalTime.AddTransitionFrom(primaryAttackNormalTime);
            inputHandler.OnPrimaryFireStarted += () => idleNormalTime.AddEventTransition(to: primaryAttackNormalTime);
            inputHandler.OnPrimaryFireStopped += () => primaryAttackNormalTime.AddEventTransition(to: idleNormalTime);

            idleNormalTime.AddTransitionTo(  secondaryAttackNormalTime);
            idleNormalTime.AddTransitionFrom(secondaryAttackNormalTime);
            inputHandler.OnSecondaryFireStarted += () => idleNormalTime.AddEventTransition(to: secondaryAttackNormalTime);
            inputHandler.OnSecondaryFireStopped += () => secondaryAttackNormalTime.AddEventTransition(to: idleNormalTime);

            #endregion

            #region Walk Normal Time

            walkNormalTime.AddTransitionTo(  idleNormalTime);
            walkNormalTime.AddTransitionFrom(idleNormalTime);
            inputHandler.OnMoveStarted += moveInput => walkNormalTime.AddEventTransition(to: idleNormalTime);
            inputHandler.OnMoveStopped += moveInput => idleNormalTime.AddEventTransition(to: walkNormalTime);

            walkNormalTime.AddTransitionTo(  dashNormalTime);
            walkNormalTime.AddTransitionFrom(dashNormalTime);
            inputHandler.OnDashTriggered += () => walkNormalTime.AddEventTransition(to: dashNormalTime);
            //inputHandler.OnDashStopped += () => dashNormalTime.AddEventTransition(to: walkNormalTime);
            
            walkNormalTime.AddTransitionTo(  primaryAttackNormalTime);
            walkNormalTime.AddTransitionFrom(primaryAttackNormalTime);
            inputHandler.OnPrimaryFireStarted += () => walkNormalTime.AddEventTransition(to: primaryAttackNormalTime);
            inputHandler.OnPrimaryFireStopped += () => primaryAttackNormalTime.AddEventTransition(to: walkNormalTime);
            
            walkNormalTime.AddTransitionTo(  secondaryAttackNormalTime);
            walkNormalTime.AddTransitionFrom(secondaryAttackNormalTime);
            inputHandler.OnSecondaryFireStarted += () => walkNormalTime.AddEventTransition(to: secondaryAttackNormalTime);
            inputHandler.OnSecondaryFireStopped += () => secondaryAttackNormalTime.AddEventTransition(to: walkNormalTime);

            #endregion
            
            #region Primary Normal Time <-> Secondary Normal Time
            
            primaryAttackNormalTime.AddTransitionTo(  secondaryAttackNormalTime);
            primaryAttackNormalTime.AddTransitionFrom(secondaryAttackNormalTime);
            inputHandler.OnSecondaryFireStarted += () => primaryAttackNormalTime.AddEventTransition(to: secondaryAttackNormalTime);
            inputHandler.OnSecondaryFireStopped += () => secondaryAttackNormalTime.AddEventTransition(to: primaryAttackNormalTime);
            
            #endregion
            
            #region Idle Bullet Time
            
            idleBulletTime.AddTransitionTo(  walkBulletTime);
            idleBulletTime.AddTransitionFrom(walkBulletTime);
            inputHandler.OnMoveStarted += moveInput => idleBulletTime.AddEventTransition(to: walkBulletTime);
            inputHandler.OnMoveStopped += moveInput => walkBulletTime.AddEventTransition(to: idleBulletTime);

            idleBulletTime.AddTransitionTo(  primaryAttackBulletTime);
            idleBulletTime.AddTransitionFrom(primaryAttackBulletTime);
            inputHandler.OnPrimaryFireStarted += () => idleBulletTime.AddEventTransition(to: primaryAttackBulletTime);
            inputHandler.OnPrimaryFireStopped += () => primaryAttackBulletTime.AddEventTransition(to: idleBulletTime);

            #endregion
            
            #region Walk Bullet Time
            
            walkBulletTime.AddTransitionTo(  idleBulletTime);
            walkBulletTime.AddTransitionFrom(idleBulletTime);
            inputHandler.OnMoveStarted += moveInput => walkBulletTime.AddEventTransition(to: idleBulletTime);
            inputHandler.OnMoveStopped += moveInput => idleBulletTime.AddEventTransition(to: walkBulletTime);
            
            walkBulletTime.AddTransitionTo(  primaryAttackBulletTime);
            walkBulletTime.AddTransitionFrom(primaryAttackBulletTime);
            inputHandler.OnPrimaryFireStarted += () => walkBulletTime.AddEventTransition(to: primaryAttackBulletTime);
            inputHandler.OnPrimaryFireStopped += () => primaryAttackBulletTime.AddEventTransition(to: walkBulletTime);
            
            #endregion
        }

        private void Update()
        {
            root.Update();
        }

        private void FixedUpdate()
        {
            root.FixedUpdate();
        }

        private void LateUpdate()
        {
            root.LateUpdate();
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
