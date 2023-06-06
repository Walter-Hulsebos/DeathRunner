using HFSM;
using UnityEngine;

namespace DeathRunner.Player
{
    public class PlayerStateLeaf_SecondaryAttack : StateLeaf
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("State.SecondaryAttack.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("State.SecondaryAttack.Exit");
        }
    }
}
