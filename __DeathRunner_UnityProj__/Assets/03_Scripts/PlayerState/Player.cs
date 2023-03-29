using DeathRunner.Inputs;
using EasyCharacterMovement;
using HFSM;
using UnityEngine;

namespace DeathRunner.Shared.StateMachine
{
    public sealed class Player : MonoBehaviour
    {
        //NOTE: [Walter] Make shared states possible??

        private State     root;
        
        private State     alive;
        private StateLeaf dead;

        private State     normalTime;
        private State     bulletTime;

        private StateLeaf idleNormalTime;
        private StateLeaf walkNormalTime;
        private StateLeaf dashNormalTime;
        private StateLeaf primaryNormalTime;
        private StateLeaf secondaryNormalTime;
        
        private StateLeaf idleBulletTime;
        private StateLeaf walkBulletTime;
        private StateLeaf primaryBulletTime;
        
        #region References

        [Tooltip(tooltip: "The Player following camera.")]
        [SerializeField] internal Camera playerCamera;
        
        /// <summary> Cached InputHandler component. </summary>
        [SerializeField, HideInInspector] internal InputHandler inputHandler;
        /// <summary> Cached CharacterMovement component. </summary>
        [SerializeField, HideInInspector] internal CharacterMotor motor;

        #if UNITY_EDITOR
        private void Reset()
        {
            FindPlayerCamera();
            
            FindInputHandler();
            
            FindCharacterMotor();
        }

        private void OnValidate()
        {
            if (playerCamera == null)
            {
                FindPlayerCamera();
            }
            
            if (inputHandler == null)
            {
                FindInputHandler();
            }
            
            if (motor == null)
            {
                FindCharacterMotor();
            }
        }

        private void FindPlayerCamera()
        {
            playerCamera = Camera.main;
        }
        
        private void FindInputHandler()
        {
            inputHandler = GetComponent<InputHandler>();
        }
        
        private void FindCharacterMotor()
        {
            // Cache CharacterMovement component
            motor = GetComponent<CharacterMotor>();

            // Enable default physic interactions
            motor.enablePhysicsInteraction = true;
        }
        
        #endif

        #endregion

        private void Awake()
        {
            CreateStateTree();
            CreateStateTransitions();
        }

        private void CreateStateTree()
        {
            root = new PlayerState_Root(alive, dead);
            {
                alive = new PlayerState_Alive(normalTime, bulletTime);
                {
                    normalTime = new PlayerState_NormalTime(idleNormalTime, walkNormalTime, dashNormalTime, primaryNormalTime, secondaryNormalTime);
                    {
                        idleNormalTime      = new PlayerStateLeaf_Idle();
                        walkNormalTime      = new PlayerStateLeaf_Walk();
                        dashNormalTime      = new PlayerStateLeaf_Dash();
                        primaryNormalTime   = new PlayerStateLeaf_Primary();
                        secondaryNormalTime = new PlayerStateLeaf_Secondary();
                    }
                    bulletTime = new PlayerState_BulletTime(idleBulletTime, walkBulletTime, primaryBulletTime);
                    {
                        idleBulletTime    = new PlayerStateLeaf_Idle();
                        walkBulletTime    = new PlayerStateLeaf_Walk();
                        primaryBulletTime = new PlayerStateLeaf_Primary();
                    }
                }
                dead = new PlayerStateLeaf_Dead();
            }
        }

        private void CreateStateTransitions()
        {
            alive.AddTransition(to: dead);
            alive.AddAnyTransition(dead);

            dead.AddTransition(to: alive);

            normalTime.AddTransition(to: bulletTime);
            normalTime.AddAnyTransition(bulletTime);
            
            bulletTime.AddTransition(to: normalTime);
            bulletTime.AddAnyTransition(normalTime);

            
            idleNormalTime.AddTransition(to: walkNormalTime);
            idleNormalTime.AddTransition(to: dashNormalTime);
            idleNormalTime.AddTransition(to: primaryNormalTime);
            idleNormalTime.AddTransition(to: secondaryNormalTime);

            walkNormalTime.AddTransition(to: idleNormalTime);
            walkNormalTime.AddTransition(to: dashNormalTime);
            walkNormalTime.AddTransition(to: primaryNormalTime);
            walkNormalTime.AddTransition(to: secondaryNormalTime);
            
            dashNormalTime.AddTransition(to: idleNormalTime);
            dashNormalTime.AddTransition(to: walkNormalTime);
            
            primaryNormalTime.AddTransition(to: idleNormalTime);
            primaryNormalTime.AddTransition(to: walkNormalTime);
            primaryNormalTime.AddTransition(to: secondaryNormalTime);

            secondaryNormalTime.AddTransition(to: idleNormalTime);
            secondaryNormalTime.AddTransition(to: walkNormalTime);
            secondaryNormalTime.AddTransition(to: primaryNormalTime);

            
            idleBulletTime.AddTransition(to: walkBulletTime);
            idleBulletTime.AddTransition(to: primaryBulletTime);

            walkBulletTime.AddTransition(to: idleBulletTime);
            walkBulletTime.AddTransition(to: primaryBulletTime);

            primaryBulletTime.AddTransition(to: idleBulletTime);
            primaryBulletTime.AddTransition(to: walkBulletTime);
        }

        private void Update()
        {
            root.Update();
        }

        private void FixedUpdate()
        {
            root.FixedUpdate();
        }

        private void LateUpdate()
        {
            root.LateUpdate();
        }

        private void OnEnable()
        {
            //root.OnEnable();
        }

        private void OnDisable()
        {
            //root.OnDisable();
        }

        private void OnDestroy()
        {
            //root.OnDestroy();
        }
    }
}
