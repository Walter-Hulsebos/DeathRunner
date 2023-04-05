using HFSM;
using UnityEngine;

namespace DeathRunner.Shared.StateMachine
{
    public class PlayerStateLeaf_Secondary : StateLeaf
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Secondary.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("Secondary.Exit");
        }
    }
}
