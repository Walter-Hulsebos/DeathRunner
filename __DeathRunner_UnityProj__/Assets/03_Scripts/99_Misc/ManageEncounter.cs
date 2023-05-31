using System;
using System.Collections;
using System.Collections.Generic;
using DeathRunner.EnemyAI;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game
{
    public class ManageEncounter : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private GameObject[] doors;


        private Transform player;
        private List<MeleeEnemyAI> meleeEnemies = new();
        private BoxCollider _collider;
        private int deadEnemies = 0;

        private bool canMeleeAttack = true;
            
        [SerializeField] private int attackCooldown;
        
        
        
        // Start is called before the first frame update
        void Start()
        {
            // foreach (GameObject enemy in enemies)
            // {
            //     enemy.
            // }
            foreach( GameObject enemy in enemies )
            {
                enemy.SetActive(false);
            }
            foreach( GameObject door in doors )
            {
                door.SetActive(false);
            }

            _collider = GetComponent<BoxCollider>();
            
            foreach (GameObject enemy in enemies)
            {
                if (enemy.TryGetComponent(out MeleeEnemyAI _meleeEnemyAI))
                {
                    meleeEnemies.Add(_meleeEnemyAI);
                    
                    //_meleeEnemyAI.OnDeath += 
                }
            }

            player = GameObject.FindWithTag("Player").transform;
        }

        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                foreach( GameObject enemy in enemies )
                {
                    enemy.SetActive(true);
                }
                // foreach( GameObject door in doors )
                // {
                //     door.SetActive(true);
                // }

                _collider.enabled = false;
            }
        }

        private void Update()
        {
            for (int i = 0; i < meleeEnemies.Count; i++)
            {
                meleeEnemies[i].GetTargetPos(new Vector3(
                    player.position.x + 2f * Mathf.Cos(2 * Mathf.PI * i / meleeEnemies.Count),
                    player.position.y,
                    player.position.z + 2f * Mathf.Sin(2 * Mathf.PI * i / meleeEnemies.Count)
                ));
            }


            if (canMeleeAttack)
            {
                meleeEnemies[Random.Range(0, meleeEnemies.Count)].canAttack = true;
                canMeleeAttack = false;
                StartCoroutine(EnableAttacking());
            }

            if (canMeleeAttack == false)
            {
                for (int i = 0; i < meleeEnemies.Count; i++)
                {
                  //  meleeEnemies[i].canAttack = false;
                }
            }
            
            
            //also handle attacking here go through all the enemies that can attack and tell one of them to attack
        }

        private IEnumerator EnableAttacking()
        {
            yield return new WaitForSeconds(2f);
            canMeleeAttack = true;
        }
        

        public void EnemyDied()
        {
            deadEnemies++;
            
            if (deadEnemies == enemies.Length)
            {
                foreach( GameObject door in doors )
                {
                    door.SetActive(false);
                }
            }
        }
    }
    

}
