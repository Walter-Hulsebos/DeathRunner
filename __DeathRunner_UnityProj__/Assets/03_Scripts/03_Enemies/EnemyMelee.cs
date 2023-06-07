using System;

using UnityEngine;

using Cysharp.Threading.Tasks;
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
                    // Set the destination for the NavMeshAgent to the player's position
                  //  navMeshAgent.SetDestination(_player.transform.position);

                    navMeshAgent.SetDestination(chasePos);
                    // Look at the player
                    LookAtPlayer();

                    // If the enemy is within attack distance, start attacking
                    if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance) //&& canAttack)
                    {
                        // Stop the enemy's movement
                        navMeshAgent.SetDestination(transform.position);
                        
                        // Set animator bool to indicate that the enemy is no longer chasing
                        animator.SetBool(is_chasing, false);

                        if (canAttack)
                        {
                            // Change the state to attacking
                            currentState = States.Attacking;
                        
                            // Trigger the attack animation
                            animator.SetTrigger(attack);   
                            
                            //navMeshAgent.enabled = false;
                        }

                        // Delay ending the attack
                        //   StartCoroutine(EndAttack());
                    }
                    else
                    {
                        navMeshAgent.SetDestination(player.transform.position);
                    }
                    break;

                case States.Attacking:
                    if (moveInAttack)
                    {
                        rigidbody.MovePosition(rigidbody.position + transform.forward * (moveSpeed * Time.deltaTime));
                    }

                    break;

                case States.Idle:
                    // Look at the player
                    LookAtPlayer();
                    break;
            }
        }
    }
}
