using UnityEngine;

using Cysharp.Threading.Tasks;
using GenericScriptableArchitecture;
using JetBrains.Annotations;
using UnityEngine.Serialization;

namespace DeathRunner.Enemies
{
    public class EnemyRanged : Enemy
    {
        [SerializeField] private Transform[] walkPositions;
        private Transform currentWalkPos;
        
        [HideInInspector] public bool hasPickedWalkPos = false;
        
        [FormerlySerializedAs("OnAttackAnimationFinished")] [SerializeField] protected EventReference OnAttackFinished;

        protected override void OnEnable()
        {
            base.OnEnable();

            OnAttackFinished.AddListener(OnAttackFinishedHandler);
            OnAttackFinished += OnAttackFinishedHandler;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();

            OnAttackFinished -= OnAttackFinishedHandler;
            OnAttackFinished.RemoveListener(OnAttackFinishedHandler);
        }

        // Called once per frame
        [UsedImplicitly]
        private void Update()
        {
            // Switch between different states based on the current state of the enemy
            switch (currentState)
            {
                case States.Chasing:
                   ChasingState();
                   break;

                case States.Attacking:
                    AttackingState();
                    break;

                case States.Idle:
                    IdleState();
                    break;
                case States.Dead:
                    
                    break;
            }
        }

        private void ChasingState()
        {
            // Set the destination for the NavMeshAgent to the player's position
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.SetDestination(player.transform.position);   
            }


            // Look at the player
            //  LookAtPlayer();

            // If the enemy is within attack distance, start attacking
            if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance) //&& canAttack)
            {
                // Change the state to attacking
                currentState = States.Attacking;

                // Stop the enemy's movement
                if (navMeshAgent.isOnNavMesh)
                {
                    navMeshAgent.SetDestination(transform.position);   
                }
                //navMeshAgent.isStopped = true;

                // Trigger the attack animation
                animator.SetTrigger(attack);

                // Set animator bool to indicate that the enemy is no longer chasing
                animator.SetBool(is_chasing, false);

                //   navMeshAgent.enabled = false;
                //TODO make it delay endAttack if the enemy gets attacked.
                //TODO figure out how to make attacking state only while actually attacking, then go to idle for some time
            }
        }

        private void IdleState()
        {
            // Look at the player
            LookAtPlayer();
            
            if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance - 2)
            {
                if (!hasPickedWalkPos)
                {
                    hasPickedWalkPos = true;
                    currentWalkPos = walkPositions[Random.Range(0, walkPositions.Length)];
                }

                if (navMeshAgent.isOnNavMesh)
                {
                    navMeshAgent.SetDestination(currentWalkPos.position);   
                }
                //Make enemy walk back to create distance with the player if they are too close
            }
        }

        private void AttackingState()
        {
            LookAtPlayer();
        }

        private void OnAttackFinishedHandler()
        {
            Debug.Log($"{nameof(EnemyRanged)} Attack animation finished");
            
            hasPickedWalkPos = false;
            
            ExitAttack().Forget();
        }

        // Rotate the enemy to face the player
        // private void LookAtPlayer()
        // {
        //     Vector3 dir = player.transform.position - transform.position;
        //     dir.y = 0; // keep the direction strictly horizontal
        //     Quaternion rot = Quaternion.LookRotation(dir);
        //     transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5 * Time.deltaTime);
        // }
    }
}