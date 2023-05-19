using HFSM;
using UnityEngine;

namespace DeathRunner.Player
{
    public sealed class PlayerState_Root : State
    {
        public PlayerState_Root(params StateObject[] childStates) : base(childStates)
        {
            
        }
        
        protected override void EnterState()
        {
            base.EnterState();
            
            Debug.Log("Root.Enter");
        }
        
        protected override void ExitState()
        {
            base.ExitState();
            
            Debug.Log("Root.Exit");
        }
    }
}
