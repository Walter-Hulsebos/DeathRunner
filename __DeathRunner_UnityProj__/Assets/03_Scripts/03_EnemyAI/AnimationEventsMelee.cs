using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.EnemyAI
{
    public class AnimationEventsMelee : MonoBehaviour
    {
        [SerializeField] private MeleeEnemyAI meleeEnemyAI;

        [SerializeField] private GameObject swordHitbox;
        
        //This can also work for combo attacks, call enableattackmove when the character starts slashing, disableattackmove, when he stops slashing, and End Attack
        // when the entire string of attacks is done. these can also be used to handle hitboxes

        private void Start()
        {
            swordHitbox.SetActive(false);
        }

        public void EnableAttackMove()
        {
            meleeEnemyAI.moveInAttack = true;
            swordHitbox.SetActive(true);
        }
        
        public void DisableAttackMove()
        {
            meleeEnemyAI.moveInAttack = false;
            swordHitbox.SetActive(false);
        }
        
        public void FinishAttack()
        {
            meleeEnemyAI.ExitAttack();
        }
    }
}
