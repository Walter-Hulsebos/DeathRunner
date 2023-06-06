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
    public abstract class EnemyAI : MonoBehaviour
    {
        // Define possible states for the enemy AI
        public enum States
        {
            Attacking,
            Chasing,
            Idle,
            Dead
        }

        // Reference to the player GameObject
        protected GameObject _player;

        // Reference to the NavMeshAgent component
        protected NavMeshAgent navMeshAgent;

        protected Rigidbody rigidbody;

        // Current state of the enemy AI
        protected States currentState;

        // Distance at which the enemy will start attacking the player
        [SerializeField] protected float attackDistance = 2;

        // Animator component for the enemy
        [SerializeField] protected Animator animator;

        protected bool canAttack;

        // Cooldown between enemy attacks
        public float attackCooldown = 1;

        protected bool moveInAttack = false;

        protected Vector3 chasePos = Vector3.zero;

        // Called once when the object is created
        protected virtual void Start()
        {
            // Find the player object in the scene
            _player = GameObject.FindWithTag("Player");

            // Start chasing the player
            StartChase();
        }

        [SerializeField] protected GameObject healthDrop;

        // Start chasing the player
        public virtual void StartChase()
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
        protected async UniTask Update()
        {
            // Switch between different states based on the current state of the enemy
            switch (currentState)
            {
                case States.Chasing:
                    // Set the destination for the NavMeshAgent to the player's position
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

            if (vectorToPlayer == Vector3.zero) return;

            Vector3 vectorToPlayerFlattened = new Vector3(vectorToPlayer.x, 0, vectorToPlayer.z);
            Vector3 directionToPlayer = vectorToPlayerFlattened.normalized;

            if (directionToPlayer == Vector3.zero) return;

            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        
        public void OnDeathHandler()
        {
            //TODO: Redo this.
            // foreach (Transform __child in transform)
            // {
            //     __child.gameObject.layer = LayerMask.NameToLayer("Pickup");
            //     print("changin layer");
            // }
            
            StopAllCoroutines();
            navMeshAgent.SetDestination(transform.position);
            //navMeshAgent.velocity = Vector3.zero;
            currentState = States.Dead;
            navMeshAgent.velocity = Vector3.zero;
            Instantiate(healthDrop, position: transform.position, rotation: Quaternion.identity);
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

        // // Method to handle enemy taking damage
        // public virtual void TakeDamage(int damage)
        // {
        //     // Reduce enemy's health by the damage amount
        //     health -= damage;
        //
        //     // Trigger the hit animation
        //     animator.SetTrigger("hit");
        //
        //     // Check if the enemy's health is depleted
        //     if (health <= 0)
        //     {
        //         // Change the state to dead
        //         currentState = States.Dead;
        //
        //         // Trigger the death animation
        //         animator.SetTrigger("dead");
        //
        //         // Disable the NavMeshAgent component
        //         navMeshAgent.enabled = false;
        //
        //         // Disable this enemy AI script
        //         enabled = false;
        //
        //         // Start coroutine to delay the destruction of the enemy object
        //         StartCoroutine(DestroyEnemy());
        //     }
        // }

        // Coroutine to delay the destruction of the enemy object
        // protected IEnumerator DestroyEnemy()
        // {
        //     // Wait for the death animation to finish playing
        //     yield return new WaitForSeconds(2);
        //
        //     // Check if the enemy should drop a health pickup
        //     if (healthDrop != null)
        //     {
        //         // Instantiate the health pickup at the enemy's position
        //         Instantiate(healthDrop, transform.position, Quaternion.identity);
        //     }
        //
        //     // Destroy the enemy object
        //     Destroy(gameObject);
        // }

        // Method to handle enemy attacking the player
        // protected void AttackPlayer()
        // {
        //     // Check if the player is still within attack distance
        //     if (Vector3.Distance(transform.position, _player.transform.position) <= attackDistance)
        //     {
        //         // Trigger the attack animation
        //         animator.SetTrigger("attack");
        //
        //         // Deal damage to the player
        //         //TODO: Refactor this.
        //         //_player.GetComponent<Player>().TakeDamage(attackDamage);
        //     }
        // }

        // Method to handle enemy cooldown between attacks
        // protected IEnumerator AttackCooldown()
        // {
        //     // Disable attacking temporarily
        //     canAttack = false;
        //
        //     // Wait for the attack cooldown duration
        //     yield return new WaitForSeconds(attackCooldown);
        //
        //     // Enable attacking again
        //     canAttack = true;
        // }
    }
}