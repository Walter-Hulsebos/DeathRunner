using HFSM;
using UnityEngine;

namespace DeathRunner.Shared.StateMachine
{
    public class PlayerStateLeaf_Dash : StateLeaf
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Dash.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("Dash.Exit");
        }
    }
}
