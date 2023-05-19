using HFSM;
using UnityEngine;

namespace DeathRunner.Player
{
    public class PlayerStateLeaf_Dead : StateLeaf
    {
        protected override void EnterState()
        {
            base.EnterState();
            
            Debug.Log("Dead.Enter");
        }
        
        protected override void ExitState()
        {
            base.ExitState();
            
            Debug.Log("Dead.Exit");
        }
    }
}