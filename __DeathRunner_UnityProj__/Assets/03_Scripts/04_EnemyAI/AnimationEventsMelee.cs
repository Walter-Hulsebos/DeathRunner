using System;
using UnityEngine;

namespace DeathRunner.EnemyAI
{
    public class AnimationEventsMelee : MonoBehaviour
    {
        [SerializeField] private MeleeEnemyAI meleeEnemyAI;

        [SerializeField] private GameObject swordHitbox;
        
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
            if (meleeEnemyAI == null)
            {
                FindEnemyAI();
            }
        }
        
        private void FindEnemyAI()
        {
            //searches for enemy ai in this gameobject's parent
            meleeEnemyAI = transform.parent.GetComponent<MeleeEnemyAI>();
        }
        
        [ContextMenu("Find Sword Hitbox")]
        private void FindSwordHitbox()
        {
            //searches for sword hitbox in this gameobject's children
            swordHitbox = transform.Find("SwordHitbox").gameObject;
        }
        #endif

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
