using System;

using UnityEngine;

using JetBrains.Annotations;

namespace DeathRunner.Enemies
{
    public sealed class EnemyMelee : Enemy
    {
        private Vector3 chasePos = Vector3.zero;
        
        [SerializeField] private Boolean canAttack;
        
        [SerializeField] private bool moveInAttack = false;

        [SerializeField] private float moveSpeed = 5;

        protected override void StartChase()
        {
            base.StartChase();

            // Disable allowing attacks when the enemy starts chasing.
            canAttack = false;
        }
        
        [UsedImplicitly]
        public void SetTargetPos(Vector3 targetPos)
        {
            chasePos = targetPos;
        }
        
        [UsedImplicitly]
        private void Update()
        {
            if (navMeshAgent.velocity != Vector3.zero)
            {
                animator.SetFloat(speed, 1);
                animator.SetTrigger(is_chasing);
            }
            else
            {
                animator.SetFloat(speed, 0);
            }
            
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

        private void IdleState()
        {
            LookAtPlayer();
        }

        private void AttackingState()
        {
            if (moveInAttack)
            {
                rigidbody.MovePosition(rigidbody.position + transform.forward * (moveSpeed * Time.deltaTime));
            }
        }

        private void ChasingState()
        {
            // Set the destination for the NavMeshAgent to the player's position
            //  navMeshAgent.SetDestination(_player.transform.position);

            navMeshAgent.SetDestination(chasePos);
            // Look at the player
            LookAtPlayer();

            // If the enemy is within attack distance, start attacking
            if (canAttack && (Vector3.Distance(transform.position, player.transform.position) <= attackDistance))
            {
                // Stop the enemy's movement
                navMeshAgent.SetDestination(transform.position);

                // Set animator bool to indicate that the enemy is no longer chasing
                animator.SetBool(is_chasing, false);

                if (Time.time >= TimeOfNextAttack)
                {
                    // Change the state to attacking
                    currentState = States.Attacking;

                    // Trigger the attack animation
                    animator.SetTrigger(attack);
                    _timeOfLastAttack = Time.time;

                    //navMeshAgent.enabled = false;
                }

                // Delay ending the attack
                //   StartCoroutine(EndAttack());
            }
            else
            {
                navMeshAgent.SetDestination(player.transform.position);
            }
        }
    }
}
