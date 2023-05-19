using HFSM;
using UnityEngine;

namespace DeathRunner.Player
{
    public class PlayerState_BulletTime : State
    {
        public PlayerState_BulletTime(params StateObject[] childStates) : base(childStates: childStates) { }
        
        protected override void EnterState()
        {
            base.EnterState();
            
            Debug.Log("BulletTime.Enter");
            
            //Commands.IsSlowMotionEnabled = true;
        }
        
        protected override void ExitState()
        {
            base.ExitState();
            
            Debug.Log("BulletTime.Exit");
        }
    }
}
