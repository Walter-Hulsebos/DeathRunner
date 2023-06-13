using System;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using UnityEngine;

namespace DeathRunner.EnemyAI
{
    public class AnimationEventsBoss : MonoBehaviour
    {
        private Boss boss;

        [SerializeField] private GameObject swordHitbox;

        [SerializeField] private MMF_Player feedbacks;

        [SerializeField] private GameObject aoePrefab;
        //This can also work for combo attacks, call enableattackmove when the character starts slashing, disableattackmove, when he stops slashing, and End Attack
        // when the entire string of attacks is done. these can also be used to handle hitboxes
        
        #if UNITY_EDITOR
        private void Reset()
        {
            FindEnemyAI();
            FindSwordHitbox();
        }
        
        private void OnValidate()
        {
            if (swordHitbox == null)
            {
                FindSwordHitbox();
            }
            if (boss == null)
            {
                FindEnemyAI();
            }
        }
        
        private void FindEnemyAI()
        {
            //searches for enemy ai in this gameobject's parent
            boss = transform.parent.GetComponent<Boss>();
        }
        
        [ContextMenu("Find Sword Hitbox")]
        private void FindSwordHitbox()
        {
            //searches for sword hitbox in this gameobject's children
            if (transform != null)
            {
                Transform foundHitBox = transform.Find("SwordHitbox");
                
                if (foundHitBox != null)
                {
                    swordHitbox = foundHitBox.gameObject;
                }
            }
        }
        #endif

        private void Start()
        {
            swordHitbox.SetActive(false);
        }

        public void EnableAttackMove()
        {
            boss.moveInAttack = true;
            swordHitbox.SetActive(true);
            feedbacks.PlayFeedbacks();
        }
        
        public void DisableAttackMove()
        {
            boss.moveInAttack = false;
            swordHitbox.SetActive(false);
        }

        public void AOEAttack()
        {
            Instantiate(aoePrefab, transform.position, quaternion.identity);
        }
        public void FinishAttack()
        {
            print("EndingAttack");
            boss.ExitAttack();
        }
    }
}
