using HFSM;
using UnityEngine;

namespace DeathRunner.Shared.StateMachine
{
    public class PlayerStateLeaf_SecondaryAttack : StateLeaf
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("SecondaryAttack.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("SecondaryAttack.Exit");
        }
    }
}
