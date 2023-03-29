using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.EnemyAI
{
    public class AnimationEventsRanged : MonoBehaviour
    {
        [SerializeField] private RangedEnemyAI rangedEnemyAI;
        
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform shootPos;
        
        //these can also be used to handle hitboxes
        public void Attack()
        {
            Instantiate(bulletPrefab,shootPos.position, transform.rotation);
        }

        public async UniTask EndAttack()
        {
            rangedEnemyAI.FinishAttack();
            // rangedEnemyAI.currentState = RangedEnemyAI.States.Idle; 
            // await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false);
            // rangedEnemyAI.StartChase();
            // rangedEnemyAI.hasPickedWalkPos = false;
        }
    }
}