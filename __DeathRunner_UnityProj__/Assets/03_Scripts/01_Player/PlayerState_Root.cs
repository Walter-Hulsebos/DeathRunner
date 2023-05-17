using HFSM;
using UnityEngine;

namespace DeathRunner.Player
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
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("Root.Exit");
        }
    }
}
