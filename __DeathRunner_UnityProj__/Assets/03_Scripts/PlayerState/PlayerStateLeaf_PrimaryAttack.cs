using HFSM;
using UnityEngine;

namespace DeathRunner.Shared.StateMachine
{
    public class PlayerStateLeaf_PrimaryAttack : StateLeaf
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("PrimaryAttack.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("PrimaryAttack.Exit");
        }
    }
}
