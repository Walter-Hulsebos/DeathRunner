using HFSM;
using UnityEngine;

namespace DeathRunner.Player
{
    public sealed class PlayerState_Alive : State
    {
        public PlayerState_Alive(params StateObject[] childStates) : base(childStates: childStates)
        {
            
        }
        
        protected override void OnEnter()
        {
            base.OnEnter();
            
            //Debug.Log("Alive.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            //Debug.Log("Alive.Exit");
        }
    }
}
