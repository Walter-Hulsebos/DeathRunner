using System.Collections;
using System.Numerics;
using Cysharp.Threading.Tasks;

using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace DeathRunner.EnemyAI
    {
        public class MeleeEnemyAI : MonoBehaviour
        {
            // Define possible states for the enemy AI
            [SerializeField] public enum States
            {
                Attacking,
                Chasing,
                Idle,
                Dead
            }

            // Reference to the player GameObject
            private GameObject _player;

            // Reference to the NavMeshAgent component
            [SerializeField] private NavMeshAgent navMeshAgent;

            private Rigidbody rigidbody;

            // Current state of the enemy AI
            [HideInInspector] public States currentState;

            // Distance at which the enemy will start attacking the player
            [SerializeField] private float attackDistance = 2;

            // Animator component for the enemy
            [SerializeField] private Animator animator;

            [HideInInspector] public bool canAttack;

            // Cooldown between enemy attacks
            public float attackCooldown = 1;

            [HideInInspector] public bool moveInAttack = false;


            private Vector3 chasePos = Vector3.zero;
            
            // Called once when the object is created
            private void Start()
            {
                // Find the player object in the scene
                _player = GameObject.FindWithTag("Player");

                // Start chasing the player
                StartChase();
            }

            [SerializeField] private GameObject healthDrop;
            
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
                      //  navMeshAgent.SetDestination(_player.transform.position);

                        navMeshAgent.SetDestination(chasePos);

                        // Look at the player
                        LookAtPlayer();

                        // If the enemy is within attack distance, start attacking
                        if (Vector3.Distance(transform.position, _player.transform.position) <= attackDistance && canAttack)
                        {
                            // Change the state to attacking
                            currentState = States.Attacking;

                            // Stop the enemy's movement
                            navMeshAgent.SetDestination(transform.position);

                            // Trigger the attack animation
                            animator.SetTrigger("attack");

                            // Set animator bool to indicate that the enemy is no longer chasing
                            animator.SetBool("isChasing", false);

                            // Delay ending the attack
                         //   StartCoroutine(EndAttack());
                            
                           
                            navMeshAgent.enabled = false;
                        }
                        break;

                    case States.Attacking:
                        if (moveInAttack)
                        {
                            rigidbody.MovePosition(rigidbody.position + transform.forward * Time.deltaTime * 5f);
                        }

                        break;

                    case States.Idle:
                        // Look at the player
                        LookAtPlayer();
                        break;
                }
            }

            // Rotate the enemy to face the player
            private void LookAtPlayer()
            {
                Vector3 vectorToPlayer = (_player.transform.position - transform.position);
                
                if(vectorToPlayer == Vector3.zero) return;
                
                Vector3 vectorToPlayerFlattened = new Vector3(vectorToPlayer.x, 0, vectorToPlayer.z);
                Vector3 directionToPlayer = vectorToPlayerFlattened.normalized;
                
                if(directionToPlayer == Vector3.zero) return;
                
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            //TODO delete this or do it better, this is managed by animation events now
            // Delay ending the attack
            public IEnumerator EndAttack(float waitTime)
            {
                yield return new WaitForSeconds(waitTime);
                StartChase();
            }

            public void OnTakeDamage()
            {
                animator.SetTrigger("Stun");
                StopAllCoroutines();
                navMeshAgent.velocity = Vector3.zero;
                navMeshAgent.SetDestination(transform.position);
                transform.LookAt(_player.transform.position);
                ExitAttack();
            }
            
            public void ExitAttack()
            {
                currentState = States.Idle;
                //TODO make it have different time if the attack finishes naturally, and if you get stunned mid attack
                StartCoroutine(EndAttack(1));
            }
            
            public void OnDeath()
            {
                
                foreach(Transform child in transform.GetComponentsInChildren<Transform>() )
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Pickup");
                    print("changin layer");
                }
                StopAllCoroutines();
                navMeshAgent.SetDestination(transform.position);
                navMeshAgent.velocity = Vector3.zero;
                currentState = States.Dead;
                navMeshAgent.velocity = Vector3.zero;
                Instantiate(healthDrop, transform.position, quaternion.identity);
                StopAllCoroutines();
                animator.SetTrigger("Death");
            }

            public void GetTargetPos(Vector3 targetPos)
            {
                chasePos = targetPos;
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