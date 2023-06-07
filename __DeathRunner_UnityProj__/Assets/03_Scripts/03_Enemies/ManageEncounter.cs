using System;
using System.Collections;
using System.Collections.Generic;
using DeathRunner.EnemyAI;
using UnityEngine;
using UnityEngine.AI;

using Cinemachine;

using Random = UnityEngine.Random;

using DeathRunner;
using MoreMountains.Feedbacks;

namespace Game
{
    public class ManageEncounter : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private GameObject[] doors;

        private CinemachineTargetGroup _cinemachineTargetGroup;
        private EncounterManagerMain encounterManagerMain;

        private Transform player;
        private List<MeleeEnemyAI> meleeEnemies = new();
        private List<RangedEnemyAI> rangedEnemies = new();
        private BoxCollider _collider;
        private int deadEnemies = 0;

        private bool canMeleeAttack = true;
        private bool canRangedAttack = true;
        
        [SerializeField] private int attackCooldown;

        [SerializeField] private MMFeedbacks _feedbacks;
        
        
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
                
                if (enemy.TryGetComponent(out RangedEnemyAI _rangedEnemy))
                {
                    rangedEnemies.Add(_rangedEnemy);
                    
                    //_meleeEnemyAI.OnDeath += 
                }
            }

            player = GameObject.FindWithTag("Player").transform;

            encounterManagerMain = GameObject.FindWithTag("EncounterManagerMain").GetComponent<EncounterManagerMain>();
            _cinemachineTargetGroup = GameObject.FindWithTag("TargetGroup").GetComponent<CinemachineTargetGroup>();
            
        }

        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                foreach( GameObject enemy in enemies )
                {
                    enemy.SetActive(true);
                    _cinemachineTargetGroup.AddMember(enemy.transform, 0.25f, 2);
                }
                encounterManagerMain.mainCam.SetActive(false);
                
                 foreach( GameObject door in doors )
                 {
                     door.SetActive(true);
                 }

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


            if (canMeleeAttack && meleeEnemies.Count > 0)
            {
                MeleeEnemyAI enemy = meleeEnemies[Random.Range(minInclusive: 0, maxExclusive: meleeEnemies.Count)];
                if (enemy != null)
                {
                    enemy.canAttack = true;   
                }
                StartCoroutine(EnableAttacking());
            }
            
            
            if (canRangedAttack && meleeEnemies.Count > 0)
            {
                rangedEnemies[Random.Range(0, meleeEnemies.Count)].canAttack = true;
                canRangedAttack = false;
                StartCoroutine(EnableRangedAttacking());
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
            canMeleeAttack = false;
            yield return new WaitForSeconds(2f);
            canMeleeAttack = true;
        }

        private IEnumerator EnableRangedAttacking()
        {
            yield return new WaitForSeconds(1f);
            canRangedAttack = true;
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

                foreach (GameObject enemy in enemies)
                {
                    _cinemachineTargetGroup.RemoveMember(enemy.transform);
                }
                _feedbacks.PlayFeedbacks();
                encounterManagerMain.cinematicCam.SetActive(true);
                StartCoroutine(EnableCamera());
            }
        }

        private IEnumerator EnableCamera()
        {
            yield return new WaitForSeconds(0.5f);
            encounterManagerMain.cinematicCam.SetActive(false);
            encounterManagerMain.mainCam.SetActive(true);
        }
    }
    

}
