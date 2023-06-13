using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using GenericScriptableArchitecture;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

using F32 = System.Single;

namespace DeathRunner
{
    public class Boss : MonoBehaviour
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
            
            [SerializeField] private EventReference OnHealthDepleted;
            
            [SerializeField] private EventReference<F32, F32> OnHealthDecreased;

            private int timesHit = 0;

            public Image healthImage;

            private bool canBeStunlocked = true;
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

            private void OnHealthDepletedHandler()
            {
                OnDeath();
            }

            private void OnHealthDecreasedHandler(F32 arg1, F32 arg2)
            {
                OnTakeDamage();
            }

            
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
                if (navMeshAgent.velocity != Vector3.zero)
                {
                    animator.SetFloat("Speed", 1);
                    animator.SetTrigger("isChasing");
                }
                else
                {
                    animator.SetFloat("Speed", 0);
                }
                // Switch between different states based on the current state of the enemy
                switch (currentState)
                {
                    case States.Chasing:
                        // Set the destination for the NavMeshAgent to the player's position
                        navMeshAgent.SetDestination(_player.transform.position);

                      //  navMeshAgent.SetDestination(chasePos);

                        // Look at the player
                        LookAtPlayer();

                        // If the enemy is within attack distance, start attacking
                        if (Vector3.Distance(transform.position, _player.transform.position) <= attackDistance /*&& canAttack*/)
                        {
                            // Change the state to attacking
                            currentState = States.Attacking;

                            // Stop the enemy's movement
                            navMeshAgent.SetDestination(transform.position);

                            int attackNum = Random.Range(0, 3); 
                             if ( attackNum == 1) 
                             {
                            // Trigger the attack animation
                            animator.SetTrigger("attack");
                            }
                             else if (attackNum == 2)
                             {
                                 animator.SetTrigger("attack2");
                             }
                             else
                             {
                                 animator.SetTrigger("attack3");
                             }
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
                Vector3 dir = _player.transform.position - transform.position;
                dir.y = 0; // keep the direction strictly horizontal
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5 * Time.deltaTime);
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
              //  healthImage.fillAmount =;
                if (timesHit <= 2)
                {
                    timesHit++;
                    animator.SetTrigger("Stun");
                    StopAllCoroutines();
                    navMeshAgent.velocity = Vector3.zero;
                    navMeshAgent.SetDestination(transform.position);
                    transform.LookAt(_player.transform.position);
                    ExitAttack();
                }
                else if (canBeStunlocked)
                {
                    canBeStunlocked = false;
                    StartCoroutine(EnableStunlock());
                }
            }

            IEnumerator EnableStunlock()
            {
                yield return new WaitForSeconds(4);
                timesHit = 0;
                canBeStunlocked = true;
            }
            public void ExitAttack()
            {
                currentState = States.Idle;
                //TODO make it have different time if the attack finishes naturally, and if you get stunned mid attack
                StartCoroutine(EndAttack(0.25f));
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
                animator.SetTrigger("Death");
                print("Am Dead");
                enabled = false;
                navMeshAgent.enabled = false;
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
                navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            }

            private void FindRB()
            {
                rigidbody = GetComponent<Rigidbody>();
            }
#endif
        }
    }
