using HFSM;
using UnityEngine;

namespace DeathRunner.Player
{
    public class PlayerStateLeaf_Dead : StateLeaf
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Dead.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("Dead.Exit");
        }
    }
}