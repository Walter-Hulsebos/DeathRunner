using HFSM;
using UnityEngine;

namespace DeathRunner.Shared.StateMachine
{
    public class PlayerState_BulletTime : State
    {
        public PlayerState_BulletTime(params StateObject[] childStates) : base(childStates: childStates) { }
        
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("BulletTime.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("BulletTime.Exit");
        }
    }
}
