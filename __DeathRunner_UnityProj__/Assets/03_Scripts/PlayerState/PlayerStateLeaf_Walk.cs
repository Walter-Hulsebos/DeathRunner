using HFSM;
using UnityEngine;

namespace DeathRunner.Shared.StateMachine
{
    public sealed class PlayerStateLeaf_Walk : StateLeaf
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Walk.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("Walk.Exit");
        }
    }
}