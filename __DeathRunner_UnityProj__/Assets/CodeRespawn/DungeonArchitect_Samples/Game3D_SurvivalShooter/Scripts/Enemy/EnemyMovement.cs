//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;
using UnityEngine.AI;

namespace DungeonArchitect.Samples.ShooterGame
{
    public class EnemyMovement : MonoBehaviour
    {
        private Transform player;               // Reference to the player's position.
        private PlayerHealth playerHealth;      // Reference to the player's health.
        private EnemyHealth enemyHealth;        // Reference to this enemy's health.
        private NavMeshAgent navAgent;

        private void Awake ()
        {
            // Set up the references.
            player = GameObject.FindGameObjectWithTag ("Player").transform;
            playerHealth = player.GetComponent <PlayerHealth> ();
            enemyHealth = GetComponent <EnemyHealth> ();
			navAgent = GetComponent<NavMeshAgent>();
        }

        private void Update ()
        {
            // If the enemy and the player have health left...
            if(enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
            {
                // ... set the destination of the nav mesh agent to the player.
				navAgent.destination = player.position;
            }
            // Otherwise...
            else
            {
                // ... disable the nav mesh agent.
				navAgent.enabled = false;
            }
        }
    }
}