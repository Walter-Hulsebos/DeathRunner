using System.Collections;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace DeathRunner.EnemyAI
{
    public class RangedEnemyAI : MonoBehaviour
    {
        // Define possible states for the enemy AI
        public enum States
        {
            Attacking,
            Chasing,
            Idle
        }

        // Reference to the player GameObject
        private GameObject _player;

        // Reference to the NavMeshAgent component
        private NavMeshAgent navMeshAgent;

        private Rigidbody rigidbody;

        // Current state of the enemy AI
        [HideInInspector] public States currentState;

        // Distance at which the enemy will start attacking the player
        [SerializeField] private float attackDistance = 10;

        // Animator component for the enemy
        [SerializeField] private Animator animator;

        // Cooldown between enemy attacks
        public float attackCooldown = 1;

        [HideInInspector] public bool moveInAttack = false;

        [SerializeField] private Transform[] walkPositions;

        private Transform currentWalkPos;
        [HideInInspector] public bool hasPickedWalkPos = false;
        
        
        private void Start()
        {
            // Find the player object in the scene
            _player = GameObject.FindWithTag("Player");

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
            
        }

        [UsedImplicitly]
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
                    if (Vector3.Distance(transform.position, _player.transform.position) <= attackDistance)
                    {
                        // Change the state to attacking
                        currentState = States.Attacking;

                        // Stop the enemy's movement
                        navMeshAgent.SetDestination(transform.position);

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
                            currentWalkPos = walkPositions[Random.Range(0, walkPositions.Length + 1)];
                        }
                        Debug.Log("ranged enemy moving");
                        navMeshAgent.SetDestination(currentWalkPos.position);
                        //Make enemy walk back to create distance with the player if they are too close
                    }
                    break;
            }
        }

        // Rotate the enemy to face the player
        private void LookAtPlayer()
        {
            Debug.Log("islookingatplayer");
            
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