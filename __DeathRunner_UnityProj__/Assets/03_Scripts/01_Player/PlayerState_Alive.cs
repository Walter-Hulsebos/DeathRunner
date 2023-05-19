using HFSM;
using UnityEngine;

namespace DeathRunner.Player
{
    public sealed class PlayerState_Alive : State
    {
        public PlayerState_Alive(params StateObject[] childStates) : base(childStates: childStates)
        {
            
        }
        
        protected override void EnterState()
        {
            base.EnterState();
            
            Debug.Log("Alive.Enter");
        }
        
        protected override void ExitState()
        {
            base.ExitState();
            
            Debug.Log("Alive.Exit");
        }
    }
}
