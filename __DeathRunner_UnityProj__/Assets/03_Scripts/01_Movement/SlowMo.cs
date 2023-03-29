using System.Collections;

using UnityEngine;
using static Unity.Mathematics.math;

using EasyCharacterMovement;
using JetBrains.Annotations;
using UltEvents;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using static ProjectDawn.Mathematics.math2;

using DeathRunner.Inputs;
using DeathRunner.Shared;
using DeathRunner.Utils;
using ProjectDawn.Mathematics;
using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

namespace DeathRunner.Movement
{
    public sealed class SlowMo : Module
    {
        #region Variables
        
        [Tooltip(tooltip: "The Player following camera.")]
        [SerializeField] private Camera playerCamera;

        [Space(height: 15f)]
        
        [Tooltip(tooltip: "The character's maximum speed. (m/s)")]
        #if ODIN_INSPECTOR
        [SuffixLabel(label: "m/s", overlay: true)]
        #endif
        [SerializeField] private F32 maxSpeed = 3.5f;

        [Tooltip(tooltip: "Max Acceleration (rate of change of velocity).")]
        [SerializeField] private F32 maxAcceleration = 10.0f;

        [SerializeField] private F32 moveDirectionSmoothingSpeed = 0.2f;

        [Tooltip(tooltip: "Setting that affects movement control. Higher values allow faster changes in direction.")]
        [SerializeField] private F32 groundFriction = 5.0f;

        [Tooltip(tooltip: "Friction to apply when falling.")]
        [SerializeField] private F32 airFriction = 0.05f;

        [Range(min: 0.0f, max: 1.0f)]
        [Tooltip(tooltip: "When falling, amount of horizontal movement control available to the character.\n" +
                          "0 = no control, 1 = full control at max acceleration.")]
        [SerializeField] private F32 airControl = 0.3f;

        [Tooltip(tooltip: "The character's gravity.")]
        [SerializeField] private F32x3 gravity = new F32x3(x: 0, y: -10, z: 0);

        private Coroutine _lateFixedUpdateCoroutine;

        /// <summary> Cached InputHandler component. </summary>
        [SerializeField, HideInInspector] private InputHandler inputHandler;
        /// <summary> Cached CharacterMovement component. </summary>
        [SerializeField, HideInInspector] private CharacterMotor motor;

        #endregion

        #region Events

        #if ODIN_INSPECTOR
        [field: FoldoutGroup(groupName: "Events", expanded: false)]
        #endif
        [field: SerializeField] public UltEvent OnLanded { get; [UsedImplicitly] private set; } = new UltEvent();
        
        #if ODIN_INSPECTOR
        [field: FoldoutGroup(groupName: "Events", expanded: false)]
        #endif
        [field: SerializeField] public UltEvent<F32x3> OnMove { get; [UsedImplicitly] private set; } = new UltEvent<F32x3>();

        #endregion

        #region EVENT HANDLERS
        
        /// <summary>
        /// FoundGround event handler.
        /// </summary>
        private void OnFoundGround(ref FindGroundResult foundGround)
        {
            //Debug.Log(message: "Found ground...");

            // Determine if the character has landed
            if (!motor.wasOnGround && foundGround.isWalkableGround)
            {
                OnLanded?.Invoke();
            }
        }

        #endregion

        #region Methods

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

        private void OnEnable()
        {
            EnableLateFixedUpdate();

            // Subscribe to CharacterMovement events
            motor.FoundGround += OnFoundGround;
            //motor.Collided    += OnCollided;
        }

        private void EnableLateFixedUpdate()
        {
            if (_lateFixedUpdateCoroutine != null)
            {
                StopCoroutine(routine: _lateFixedUpdateCoroutine);
            }
            _lateFixedUpdateCoroutine = StartCoroutine(LateFixedUpdate());
        }

        private void OnDisable()
        {
            DisableLateFixedUpdate();

            // Un-Subscribe from CharacterMovement events
            motor.FoundGround -= OnFoundGround;
            //motor.Collided    -= OnCollided;
        }

        private void DisableLateFixedUpdate()
        {
            if (_lateFixedUpdateCoroutine != null)
            {
                StopCoroutine(routine: _lateFixedUpdateCoroutine);
            }
        }

        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
        private IEnumerator LateFixedUpdate()
        {
            while (true)
            {
                yield return _waitForFixedUpdate;

                OnLateFixedUpdate();
            }
        }
        /// <summary>
        /// Post-Physics update, used to sync our character with physics.
        /// </summary>
        private void OnLateFixedUpdate()
        {
            //UpdateRotation();
            Move();
        }
        
        private F32x3 _oldMoveDirection = F32x3.zero;
        private F32x3 _moveDirectionVelocity;

        /// <summary>
        /// Perform character movement.
        /// </summary>
        private void Move()
        {
            // Create a Movement direction vector in world space. For example: (x: 0, y: 0, z: 1) is forward, (x: 0.7071068f, y: 0f, z: 0.7071068f) is diagonally forward-right.
            F32x3 __newMoveDirection = inputHandler.MoveInputFlat;
            //__newMoveDirection.SetMaxLength(1.0f);
            
            // Make sure that if it's close to zero, it's zero.
            // if (lengthsq(__newMoveDirection) < 0.001f)
            // {
            //     __newMoveDirection = F32x3.zero;
            // }

            F32x3 __moveDirection = __newMoveDirection;
            
            if (lengthsq(__newMoveDirection) > 0.001f) //Smoothly interpolate if we're not trying to stand still.
            {
                __moveDirection = _oldMoveDirection.SmoothDamp(target: __newMoveDirection, currentVelocity: ref _moveDirectionVelocity, smoothTime: moveDirectionSmoothingSpeed, maxSpeed: 1000);
            }

            _oldMoveDirection = __moveDirection;

            // Make movementDirection relative to camera view direction
            F32x3 __moveDirectionRelativeToCamera = math2.RelativeTo(__moveDirection, playerCamera.transform);
            
            F32x3 __desiredVelocity = (__moveDirectionRelativeToCamera * maxSpeed);

            // Update character’s velocity based on its grounding status
            if (motor.isGrounded)
            {
                GroundedMovement(desiredVelocity: __desiredVelocity);
            }
            else
            {
                NotGroundedMovement(desiredVelocity: __desiredVelocity);
            }

            OnMove?.Invoke(__moveDirectionRelativeToCamera);
            
            // Perform movement using character's current velocity
            motor.Move();
        }
        
        
        /// <summary>
        /// Move the character when on walkable ground.
        /// </summary>
        private void GroundedMovement(Vector3 desiredVelocity)
        {
            motor.velocity = Vector3.Lerp(
                a: motor.velocity, 
                b: desiredVelocity,
                t: 1f - exp(-groundFriction * Time.unscaledDeltaTime));
        }

        /// <summary>
        /// Move the character when falling or on not-walkable ground.
        /// </summary>
        private void NotGroundedMovement(F32x3 desiredVelocity)
        {
            // Current character's velocity

            F32x3 __velocity = motor.velocity;

            // If moving into non-walkable ground, limit its contribution.
            // Allow movement parallel, but not into it because that may push us up.
            if (motor.isOnGround && dot(desiredVelocity, motor.groundNormal) < 0.0f)
            {
                F32x3 __groundNormal   = motor.groundNormal;

                F32x3 __planeNormal = normalize(new F32x3(x: __groundNormal.x, y: 0, z: __groundNormal.y));

                desiredVelocity = math2.ProjectedOnPlane(desiredVelocity, planeNormal: __planeNormal);
            }

            // If moving...
            if (all(desiredVelocity != F32x3.zero))
            {
                F32x3 __flatVelocity = new F32x3(x: __velocity.x, y: 0,            z: __velocity.z);
                F32x3 __verVelocity  = new F32x3(x: 0,            y: __velocity.y, z: 0);

                // Accelerate horizontal velocity towards desired velocity
                F32x3 __horizontalVelocity = Vector3.MoveTowards(
                    current: __flatVelocity, 
                    target: desiredVelocity,
                    maxDistanceDelta: maxAcceleration * airControl * Time.unscaledDeltaTime);

                // Update velocity preserving gravity effects (vertical velocity)
                __velocity = __horizontalVelocity + __verVelocity;
            }

            // Apply gravity
            __velocity += gravity * Time.unscaledDeltaTime;

            // Apply Air friction (Drag)
            __velocity -= __velocity * airFriction * Time.unscaledDeltaTime;

            // Update character's velocity
            motor.velocity = __velocity;
        }


        #endregion
    }
}
