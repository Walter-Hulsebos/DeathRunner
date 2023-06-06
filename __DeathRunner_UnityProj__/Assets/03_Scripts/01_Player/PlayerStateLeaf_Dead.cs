using HFSM;
using UnityEngine;

namespace DeathRunner.Player
{
    public class PlayerStateLeaf_Dead : StateLeaf
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("State.Dead.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("State.Dead.Exit");
        }
    }
}