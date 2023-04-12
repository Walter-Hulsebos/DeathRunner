//System libraries first
using System;

//Unity-specific libraries next
using UnityEngine;
using static Unity.Mathematics.math;

//Third-party libraries next
using JetBrains.Annotations;
using HFSM;
using ProjectDawn.Mathematics;
using GenericScriptableArchitecture;

//Project-specific libraries last
using DeathRunner.Shared;
using DeathRunner.Utils;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;

namespace DeathRunner.PlayerState
{
    public sealed class PlayerStateLeaf_Walk : StateLeaf
    {
        private readonly WalkSettings     _settings;
        private readonly PlayerReferences _references;

        public PlayerStateLeaf_Walk(WalkSettings settings, PlayerReferences references)
        {
            this._settings   = settings;
            this._references = references;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Walk.Enter");
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("Walk.Exit");
        }
        
        private F32x3 _oldMoveDirection = F32x3.zero;
        private F32x3 _moveDirectionVelocity;
        
        protected override void OnLateFixedUpdate()
        {
            base.OnLateFixedUpdate();
            
            Debug.Log("Walk.LateFixedUpdate");
            
            F32x3 __targetMoveDir = _references.InputHandler.MoveInputFlat;
            
            F32x3 __moveDirection = __targetMoveDir;

            //TODO: Remove this temporary hack for expected slow-mo player movement.
            if (!Commands.IsSlowMotionEnabled)
            {
                if (lengthsq(__targetMoveDir) > EPSILON)
                {
                    __moveDirection = _oldMoveDirection.SmoothDamp(
                        target: __targetMoveDir,
                        currentVelocity: ref _moveDirectionVelocity,
                        deltaTime: Commands.DeltaTime,
                        smoothTime: _settings.MoveDirectionSmoothingSpeed,
                        maxSpeed: 100);

                    // Set the move direction's length to that of the target.
                    //__moveDirection = normalize(__moveDirection) * length(__targetMoveDir);

                    //        Debug.Log(message: $"__moveDirection: {__moveDirection}");
                }
            }

            _oldMoveDirection = __moveDirection;

            // Make movementDirection relative to camera view direction
            F32x3 __moveDirectionRelativeToCamera = __moveDirection.RelativeTo(_references.Camera.transform);
            
            F32x3 __desiredVelocity = (__moveDirectionRelativeToCamera * _settings.MaxSpeed);

            // Update character’s velocity based on its grounding status
            if (_references.Motor.isGrounded)
            {
                GroundedMovement(desiredVelocity: __desiredVelocity);
            }
            else
            {
                NotGroundedMovement(desiredVelocity: __desiredVelocity);
            }

            //OnMove?.Invoke(__moveDirectionRelativeToCamera);
            
            // Perform movement using character's current velocity
            _references.Motor.Move(deltaTime: Commands.DeltaTime);
        }
        
        /// <summary>
        /// Move the character when on walkable ground.
        /// </summary>
        private void GroundedMovement(Vector3 desiredVelocity)
        {
            Debug.Log("GroundedMovement");
            
            _references.Motor.velocity = Vector3.Lerp(
                a: _references.Motor.velocity, 
                b: desiredVelocity,
                t: 1f - exp(-_settings.GroundFriction * Commands.DeltaTime));
        }

        /// <summary>
        /// Move the character when falling or on not-walkable ground.
        /// </summary>
        private void NotGroundedMovement(F32x3 desiredVelocity)
        {
            Debug.Log("NotGroundedMovement");
            
            F32x3 __velocity = _references.Motor.velocity;

            // If moving into non-walkable ground, limit its contribution.
            // Allow movement parallel, but not into it because that may push us up.
            if (_references.Motor.isOnGround && dot(desiredVelocity, _references.Motor.groundNormal) < 0.0f)
            {
                F32x3 __groundNormal = _references.Motor.groundNormal;

                F32x3 __planeNormal  = normalize(new F32x3(x: __groundNormal.x, y: 0, z: __groundNormal.y));

                desiredVelocity = desiredVelocity.ProjectedOnPlane(planeNormal: __planeNormal);
            }

            // If moving...
            if (any(desiredVelocity != F32x3.zero))
            {
                F32x3 __flatVelocity = new(x: __velocity.x, y: 0,            z: __velocity.z);
                F32x3 __verVelocity  = new(x: 0,            y: __velocity.y, z: 0);

                // Accelerate horizontal velocity towards desired velocity
                F32x3 __horizontalVelocity = Vector3.MoveTowards(
                    current: __flatVelocity, 
                    target: desiredVelocity,
                    maxDistanceDelta: (F32)_settings.MaxAcceleration * (F32)_settings.AirControlPrimantissa * Commands.DeltaTime);

                // Update velocity preserving gravity effects (vertical velocity)
                __velocity = __horizontalVelocity + __verVelocity;
            }

            // Apply gravity
            __velocity += (F32x3)_settings.Gravity * Commands.DeltaTime;

            // Apply Air friction (Drag)
            __velocity -= __velocity * (F32)_settings.AirFriction * Commands.DeltaTime;

            // Update character's velocity
            _references.Motor.velocity = __velocity;
        }
    }
    
    [Serializable]
    public struct WalkSettings 
    {
        [field:Tooltip(tooltip: "The character's maximum speed. (m/s)")]
        [field:SerializeField] public Constant<F32>   MaxSpeed                    { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "Max Acceleration (rate of change of velocity).")]
        [field:SerializeField] public Constant<F32>   MaxAcceleration             { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public Constant<F32>   MoveDirectionSmoothingSpeed { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "Setting that affects movement control. Higher values allow faster changes in direction.")]
        [field:SerializeField] public Constant<F32>   GroundFriction              { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "Friction to apply when falling.")]
        [field:SerializeField] public Constant<F32>   AirFriction                 { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "When falling, amount of horizontal movement control available to the character.\n" +
                                "0 = no control, 1 = full control at max acceleration.")]
        [field:SerializeField] public Constant<F32>   AirControlPrimantissa       { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "The character's gravity.")] 
        [field:SerializeField] public Constant<F32x3> Gravity                     { get; [UsedImplicitly] private set; }
        
        [field:Space]
        
        [field:SerializeField] public ScriptableEvent<F32x3> OnMove               { get; [UsedImplicitly] private set; }
        
    }
}