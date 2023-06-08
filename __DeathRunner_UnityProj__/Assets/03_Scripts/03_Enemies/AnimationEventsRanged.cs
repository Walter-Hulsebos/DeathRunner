using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DeathRunner.EnemyAI
{
    public class AnimationEventsRanged : MonoBehaviour
    {
        [SerializeField] private RangedEnemyAI rangedEnemyAI;
        
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform shootPos;
        
#if UNITY_EDITOR
        private void Reset()
        {
            FindEnemyAI();
            FindShootPos();
        }
        
        private void OnValidate()
        {
            if (rangedEnemyAI == null)
            {
                FindEnemyAI();
            }
            if (shootPos == null)
            {
                FindShootPos();
            }
        }
        
        [ContextMenu("Find Enemy AI")]
        private void FindEnemyAI()
        {
            rangedEnemyAI = transform.parent.GetComponent<RangedEnemyAI>();
        }
        
        [ContextMenu("Find Shoot Pos")]
        private void FindShootPos()
        {
            //Find shoot pos in this gameobject's siblings or children
            shootPos = transform.parent.transform.Find("ShootPos");
        }
#endif
        
        //these can also be used to handle hitboxes
        public void Attack()
        {
            Instantiate(bulletPrefab,shootPos.position, transform.rotation);
        }

        public async UniTask EndAttack()
        {
            //TODO maybe this stuff should be handled in main script and just called from here
            rangedEnemyAI.FinishAttack();
            // rangedEnemyAI.currentState = RangedEnemyAI.States.Idle; 
            // await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false);
            // rangedEnemyAI.StartChase();
            // rangedEnemyAI.hasPickedWalkPos = false;
        }
    }
}
