using DeathRunner.Shared;
using HFSM;
using UnityEngine;

namespace DeathRunner.Player
{
    public sealed class PlayerState_NormalTime : State
    {
        public PlayerState_NormalTime(params StateObject[] childStates) : base(childStates: childStates) { }
        
        protected override void EnterState()
        {
            base.EnterState();
            
            Debug.Log("NormalTime.Enter");
            
            Commands.IsSlowMotionEnabled = false;
        }
        
        protected override void ExitState()
        {
            base.ExitState();
            
            Debug.Log("NormalTime.Exit");
        }
    }
}