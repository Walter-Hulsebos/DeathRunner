﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {

    public class ThrowNova : StateMachineBehaviour {
        private CastSpells cS;
    
        public float spawnDelay;
        
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            
            if(cS == null)
            {
                cS = animator.GetComponent<CastSpells>();
            }
            
            if(cS != null)
            {
               cS.ThrowNova(spawnDelay);
            }
        }
    }
}