using System;
using Cysharp.Threading.Tasks;
using GenericScriptableArchitecture;
using UnityEngine.AI;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using F32 = System.Single;
using U16 = System.UInt16;

namespace DeathRunner.Enemies
{
    using UnityEngine;

    public abstract class Enemy : MonoBehaviour
    {
        // Define possible states for the enemy AI
        public enum States
        {
            Idle,
            Attacking,
            Chasing,
            Dead
        }

        // Reference to the player GameObject
        protected static GameObject player;
        
        [SerializeField, HideInInspector] protected NavMeshAgent navMeshAgent;
        
        #if ODIN_INSPECTOR
        [ReadOnly]
        #endif
        [SerializeField] protected States currentState;

        [Tooltip("Distance at which the enemy will start attacking the player")] 
        [SerializeField] protected F32 attackDistance = 2.5f;
        
        [Tooltip("Cooldown between enemy attacks")]
        [SerializeField] protected F32 attackCooldown = 2;
        protected F32 _timeOfLastAttack = 0;
        protected F32 TimeOfNextAttack => _timeOfLastAttack + attackCooldown;
        
        [Tooltip("Animator component for the enemy")]
        [SerializeField] protected Animator animator;
        
        [SerializeField] protected new Rigidbody rigidbody;
        
        #if ODIN_INSPECTOR
        [AssetsOnly]
        #endif
        [SerializeField] protected GameObject healthDrop;

        [SerializeField] protected EventReference OnHealthDepleted;
        
        [SerializeField] protected EventReference<U16, U16> OnHealthDecreased;
        
        protected static readonly Int32 death      = Animator.StringToHash("Death");
        protected static readonly Int32 stun       = Animator.StringToHash("Stun");
        protected static readonly Int32 speed      = Animator.StringToHash("Speed");
        protected static readonly Int32 is_chasing = Animator.StringToHash("isChasing");
        protected static readonly Int32 attack     = Animator.StringToHash("attack");

        protected virtual void OnEnable()
        {
            OnHealthDepleted.AddListener(OnHealthDepletedHandler);
            OnHealthDecreased.AddListener(OnHealthDecreasedHandler);
        }
        protected virtual void OnDisable()
        {
            OnHealthDepleted.RemoveListener(OnHealthDepletedHandler);
            OnHealthDecreased.RemoveListener(OnHealthDecreasedHandler);
        }
        
        protected virtual void OnHealthDepletedHandler()
        {
            OnDeath();
        }
        
        protected virtual void OnHealthDecreasedHandler(U16 currentHealth, U16 maxHealth)
        {
            OnTakeDamage();
        }
        
        private void Start()
        {
            // Find the player object in the scene
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }

            // Start chasing the player
            StartChase();
        }

        /// <summary> Start chasing the player </summary>
        protected virtual void StartChase()
        {
            // Set current state to chasing
            currentState = States.Chasing;

            // Set animator bool to indicate that the enemy is chasing
            animator.SetBool(is_chasing, true);

            // Enable NavMeshAgent component
            navMeshAgent.enabled = true;
        }


        /// <summary> Rotate the enemy to face the player </summary>
        protected virtual void LookAtPlayer()
        {
            if (player == null) return;

            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0; // keep the direction strictly horizontal
            
            direction.Normalize();
            
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5 * Time.deltaTime);
        }


        // Handle taking damage
        protected virtual void OnTakeDamage()
        {
            StopAllCoroutines();
            animator.SetTrigger(stun);
            
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.SetDestination(transform.position);
            
            transform.LookAt(player.transform.position);
            
            ExitAttack().Forget();
        }

        // Handle death
        protected virtual void OnDeath()
        {
            // foreach(Transform child in transform.GetComponentsInChildren<Transform>() )
            // {
            //     child.gameObject.layer = LayerMask.NameToLayer("Pickup");
            //     print("changin layer");
            // }
            
            #if UNITY_EDITOR
            Debug.Log(message: "Enemy died!", context: this);
            #endif

            StopAllCoroutines();
            navMeshAgent.SetDestination(transform.position);
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.enabled = false;
            
            currentState = States.Dead;
            
            if (Random.Range(0, 3) == 1)
            {
                Instantiate(healthDrop, transform.position, Quaternion.identity);
            }

            //StopAllCoroutines();
            animator.SetTrigger(death);
        }
        
        protected async UniTask ExitAttack()
        {
            currentState = States.Idle;
            //TODO make it have different time if the attack finishes naturally, and if you get stunned mid attack
            
            //TODO delete this or do it better, this is managed by animation events now    
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
            
            StartChase();
        }

        // Editor-only code for cleaning up the script
        #if UNITY_EDITOR
        protected virtual void Reset()
        {
            FindNavMeshAgent();

            FindAnimator();
        }

        protected virtual void OnValidate()
        {
            if (navMeshAgent == null)
            {
                FindNavMeshAgent();
            }
            
            if (animator == null)
            {
                FindAnimator();
            }
            
            if (rigidbody == null)
            {
                FindRigidbody();
            }
        }

        protected virtual void FindNavMeshAgent()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        protected virtual void FindAnimator()
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        protected virtual void FindRigidbody()
        {
            rigidbody = GetComponent<Rigidbody>();
        }
        #endif
        
    }

}
