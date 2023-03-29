//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;
using UnityEngine.AI;

namespace DungeonArchitect.Samples.Navigation
{
    public class NavigationDemoNPC : MonoBehaviour
    {
        private NavMeshAgent agent;
        private CharacterController character;

        public Transform target;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (target != null)
            {
                agent.SetDestination(target.position);

                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    character.SimpleMove(agent.desiredVelocity);
                }
                else
                {
                    character.SimpleMove(Vector3.zero);
                }
            }
            
        }
    }
}
