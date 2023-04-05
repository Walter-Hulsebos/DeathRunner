using HFSM;
using UnityEngine;

namespace DeathRunner.Shared.StateMachine
{
    public class PlayerStateLeaf_Primary : StateLeaf
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Primary.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("Primary.Exit");
        }
    }
}
