using HFSM;
using UnityEngine;

namespace DeathRunner.Shared.StateMachine
{
    public sealed class PlayerState_Root : State
    {
        public PlayerState_Root(params StateObject[] childStates) : base(childStates)
        {
            
        }
        
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Root.Enter");
        }
    }
}
