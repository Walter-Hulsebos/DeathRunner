using System;
using System.Collections;
using System.Collections;
using Cysharp.Threading.Tasks;
using GenericScriptableArchitecture;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using Random = UnityEngine.Random;

namespace DeathRunner.EnemyAI
    {
        public class RangedEnemyAI : MonoBehaviour
        {
            // Define possible states for the enemy AI
            public enum States
            {
                Idle,
                Attacking,
                Chasing,
                Dead
            }

            // Reference to the player GameObject
            private GameObject _player;

            // Reference to the NavMeshAgent component
            private NavMeshAgent navMeshAgent;

            private Rigidbody rigidbody;

            // Current state of the enemy AI
            #if ODIN_INSPECTOR
            [ReadOnly] 
            #endif
            public States currentState;

            // Distance at which the enemy will start attacking the player
            [SerializeField] private float attackDistance = 14;

            // Animator component for the enemy
            [SerializeField] private Animator animator;

            // Cooldown between enemy attacks
            public float attackCooldown = 1;

            [HideInInspector] public bool moveInAttack = false;

            [SerializeField] private Transform[] walkPositions;

            private Transform currentWalkPos;
            [HideInInspector] public bool hasPickedWalkPos = false;

            [SerializeField] private GameObject HealthDrop;
            
            [HideInInspector] public bool canAttack;
            
            
            [SerializeField] private EventReference OnHealthDepleted;
            
            [SerializeField] private EventReference<ushort, ushort> OnHealthDecreased;

            private void OnEnable()
            {
                OnHealthDepleted.AddListener(OnHealthDepletedHandler);
                OnHealthDecreased.AddListener(OnHealthDecreasedHandler);
            }
            private void OnDisable()
            {
                OnHealthDepleted.RemoveListener(OnHealthDepletedHandler);
                OnHealthDecreased.RemoveListener(OnHealthDecreasedHandler);
            }
            
            private void OnHealthDecreasedHandler(UInt16 arg1, UInt16 arg2)
            {
                OnTakeDamage();
            }

            private void OnHealthDepletedHandler()
            {
                OnDeath();
            }

            
            // Called once when the object is created
            private void Start()
            {
                // Find the player object in the scene
                _player = GameObject.FindWithTag("Player");
                navMeshAgent = GetComponent<NavMeshAgent>();
                // Start chasing the player
                StartChase();
            }

            // Start chasing the player
            public void StartChase()
            {
                // Set current state to chasing
                currentState = States.Chasing;

                // Set animator bool to indicate that the enemy is chasing
                animator.SetBool("isChasing", true);

                // Enable NavMeshAgent component
                navMeshAgent.enabled = true;

                canAttack = false;
            }

            // Called once per frame
            private async UniTask Update()
            {
                // Switch between different states based on the current state of the enemy
                switch (currentState)
                {
                    case States.Chasing:
                        // Set the destination for the NavMeshAgent to the player's position
                        navMeshAgent.SetDestination(_player.transform.position);

                        // Look at the player
                      //  LookAtPlayer();

                        // If the enemy is within attack distance, start attacking
                        if (Vector3.Distance(transform.position, _player.transform.position) <= attackDistance) //&& canAttack)
                        {
                            // Change the state to attacking
                            currentState = States.Attacking;

                            // Stop the enemy's movement
                            navMeshAgent.SetDestination(transform.position);
                            //navMeshAgent.isStopped = true;

                            // Trigger the attack animation
                            animator.SetTrigger("attack");

                            // Set animator bool to indicate that the enemy is no longer chasing
                            animator.SetBool("isChasing", false);

                         //   navMeshAgent.enabled = false;
                            //TODO make it delay endAttack if the enemy gets attacked.
                            //TODO figure out how to make attacking state only while actually attacking, then go to idle for some time
                        }

                        break;

                    case States.Attacking:
                        LookAtPlayer();
                        break;

                    case States.Idle:
                        // Look at the player
                        LookAtPlayer();
                        if (Vector3.Distance(transform.position, _player.transform.position) <= attackDistance - 2)
                        {
                            if (!hasPickedWalkPos)
                            {
                                hasPickedWalkPos = true;
                                currentWalkPos = walkPositions[Random.Range(0, walkPositions.Length)];
                            }
                            navMeshAgent.SetDestination(currentWalkPos.position);
                            //Make enemy walk back to create distance with the player if they are too close
                        }
                        break;
                    case States.Dead:
                        
                        break;
                }
            }

            // Rotate the enemy to face the player
            private void LookAtPlayer()
            {

                Vector3 dir = _player.transform.position - transform.position;
                dir.y = 0; // keep the direction strictly horizontal
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5 * Time.deltaTime);
            }

            //TODO delete this or do it better, this is managed by animation events now
            // Delay ending the attack
            public IEnumerator EndAttack()
            {
                yield return new WaitForSeconds(attackCooldown);
                StartChase();
            }

            public void FinishAttack()
            { //TODO make it have different time if the attack finishes naturally, and if you get stunned mid attack
                currentState = RangedEnemyAI.States.Idle; 
                hasPickedWalkPos = false;
                StartCoroutine(EndAttack());
            }
            public void OnTakeDamage()
            {
                StopAllCoroutines();
                transform.LookAt(_player.transform.position);
                animator.SetTrigger("Stun");
                FinishAttack();
            }
            public void OnDeath()
            { 
                // foreach(Transform child in transform.GetComponentsInChildren<Transform>() )
                // {
                //     child.gameObject.layer = LayerMask.NameToLayer("Pickup");
                //     print("changin layer");
                // }
                StopAllCoroutines();
                navMeshAgent.SetDestination(transform.position);
                navMeshAgent.velocity = Vector3.zero;
                currentState = States.Dead;
                navMeshAgent.SetDestination(transform.position);
                navMeshAgent.velocity = Vector3.zero;

                if (Random.Range(0, 3) == 1)
                {
                    Instantiate(HealthDrop, transform.position, quaternion.identity);
                }

                StopAllCoroutines();
                animator.SetTrigger("Death");
            }

            // Editor-only code for cleaning up the script
#if UNITY_EDITOR
            private void Reset()
            {
                FindNavMeshAgent();
            }

            private void OnValidate()
            {
                if (navMeshAgent == null)
                {
                    FindNavMeshAgent();
                }

                if (rigidbody == null)
                {
                    FindRB();
                }
            }

            private void FindNavMeshAgent()
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }

            private void FindRB()
            {
                rigidbody = GetComponent<Rigidbody>();
            }
#endif
        }
    }