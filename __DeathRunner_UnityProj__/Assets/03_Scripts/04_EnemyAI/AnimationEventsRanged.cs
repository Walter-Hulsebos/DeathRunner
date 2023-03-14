using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DeathRunner.EnemyAI
{
    public class AnimationEventsRanged : MonoBehaviour
    {
        [SerializeField] private RangedEnemyAI rangedEnemyAI;
        
        //these can also be used to handle hitboxes
        public void Attack()
        {
     
        }

        public async UniTask EndAttack()
        {
            rangedEnemyAI.currentState = RangedEnemyAI.States.Idle; 
            await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false);
            //change this to make walk around
            rangedEnemyAI.StartChase();
            rangedEnemyAI.hasPickedWalkPos = false;
            
            Debug.Log("over");
        }
    }
}