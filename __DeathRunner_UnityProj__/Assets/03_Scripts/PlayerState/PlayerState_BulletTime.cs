using UnityEngine;

using HFSM;

using DeathRunner.Shared;

namespace DeathRunner.PlayerState
{
    public class PlayerState_BulletTime : State
    {
        public PlayerState_BulletTime(params StateObject[] childStates) : base(childStates: childStates) { }
        
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("BulletTime.Enter");
            
            //Commands.IsSlowMotionEnabled = true;
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("BulletTime.Exit");
        }
    }
}
