using HFSM;
using UnityEngine;

namespace DeathRunner.Player
{
    public class PlayerStateLeaf_SecondaryAttack : StateLeaf
    {
        protected override void EnterState()
        {
            base.EnterState();
            
            Debug.Log("SecondaryAttack.Enter");
        }
        
        protected override void ExitState()
        {
            base.ExitState();
            
            Debug.Log("SecondaryAttack.Exit");
        }
    }
}
