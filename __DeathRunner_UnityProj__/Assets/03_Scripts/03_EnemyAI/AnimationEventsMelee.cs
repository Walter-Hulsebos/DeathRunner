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

        
        //This can also work for combo attacks, call enableattackmove when the character starts slashing, disableattackmove, when he stops slashing, and End Attack
        // when the entire string of attacks is done. these can also be used to handle hitboxes
        public void EnableAttackMove()
        {
            meleeEnemyAI.moveInAttack = true;
        }
        
        public void DisableAttackMove()
        {
            meleeEnemyAI.moveInAttack = false;
        }
        
        public async UniTask EndAttack()
        {
            meleeEnemyAI.currentState = MeleeEnemyAI.States.Idle; 
            await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false);
            meleeEnemyAI.StartChase();
        }
    }
}
