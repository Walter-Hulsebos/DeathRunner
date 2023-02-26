using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.EnemyAI
{
    public class MeleeEnemyAI : MonoBehaviour
    {

        enum States
        {
            Attacking,
            Chasing,
            Idle
        }

        private NavMeshAgent navMeshAgent;
        
        
        private States currentState;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
#if UNITY_EDITOR
        private void Reset()
        {
            FindInputHandler();
        }

        private void OnValidate()
        {

            if(navMeshAgent == null)
            {
                FindInputHandler();
            }
            //do this for every single one of those.
        }

        private void FindInputHandler()
        {
            navMeshAgent.GetComponent<NavMeshAgent>();
        }
#endif
        
    }

    }
    