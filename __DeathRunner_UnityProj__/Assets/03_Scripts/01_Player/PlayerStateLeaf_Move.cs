//System libraries first

//Unity-specific libraries next
using System;
using DeathRunner.Shared;
using DeathRunner.Utils;
using GenericScriptableArchitecture;
using HFSM;
using JetBrains.Annotations;
using ProjectDawn.Geometry3D;
using ProjectDawn.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

//Third-party libraries next

//Project-specific libraries last
using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;
using Rotor = Unity.Mathematics.quaternion;

namespace DeathRunner.Player
{
    public sealed class PlayerStateLeaf_Move : StateLeaf
    {
        private readonly MoveSettings     _settings;
        private readonly PlayerReferences _references;

        public PlayerStateLeaf_Move(MoveSettings settings, PlayerReferences references)
        {
            this._settings   = settings;
            this._references = references;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            
            #if UNITY_EDITOR
            Debug.Log("State.Walk.Enter");
            #endif
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            #if UNITY_EDITOR
            Debug.Log("State.Walk.Exit");
            #endif
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            PlayerHelpers.OrientTowardsDir(references: _references, direction: _settings.OrientationLookDirection.Value, orientationSpeed: _settings.OrientationSpeed.Value, deltaTime: Time.deltaTime);
            //PlayerHelpers.OrientTowardsDirInstant(references: _references, direction: _settings.OrientationLookDirection.Value);
            //OrientTowardsLookDirection();
        }

        // public void OrientTowardsLookDirection()
        // {
        //     Plane3D __plane3D = new(normal: up(), distance: 0);
        //     
        //     F32x3 __projectedLookDirection = normalize(__plane3D.Projection(point: _settings.OrientationLookDirection.Value));
        //     
        //     if (lengthsq(__projectedLookDirection) == 0) return;
        //
        //     Rotor __targetRotation = Rotor.LookRotation(forward: __projectedLookDirection, up: up());
        //
        //     _references.Rot = slerp(q1: _references.Rot, q2: __targetRotation, t: _settings.OrientationSpeed.Value * Time.deltaTime);
        // }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            F32x3 __targetMoveVector = _references.InputHandler.MoveInputFlat;
            F32x3 __desiredVelocity  = F32x3.zero;
            F32x3 __targetMoveDirectionRelativeToCamera = F32x3.zero;

            if (any(__targetMoveVector != F32x3.zero))
            {
                F32 __targetMoveSpeed = length(__targetMoveVector) * _settings.MaxSpeed.Value;
                
                F32x3 __targetMoveDirection = normalize(__targetMoveVector);
                
                __targetMoveDirectionRelativeToCamera = __targetMoveDirection.RelativeTo(_references.Camera.transform);
                __desiredVelocity = (__targetMoveDirectionRelativeToCamera * __targetMoveSpeed);
            }

            // Update character’s velocity based on its grounding status
            if (_references.Motor.isGrounded)
            {
                _references.Motor.velocity = lerp(
                    _references.Motor.velocity, 
                    __desiredVelocity,
                    1f - exp(-_settings.GroundFriction * Time.unscaledDeltaTime));
            }
            else
            {
                _references.Motor.velocity = PlayerHelpers.NotGroundedMovement(
                    velocity:              _references.Motor.velocity,
                    desiredVelocity:       __desiredVelocity, 
                    isOnGround:            _references.Motor.isOnGround,
                    groundNormal:          _references.Motor.groundNormal, 
                    maxAcceleration:       _settings.MaxAcceleration.Value,
                    airControlPrimantissa: _settings.AirControlPrimantissa.Value,
                    airFriction:           _settings.AirFriction.Value, 
                    gravity:               _settings.Gravity.Value);
            }

            if (all(__targetMoveDirectionRelativeToCamera == F32x3.zero)) return;
            if (any(__targetMoveDirectionRelativeToCamera == F32.NaN)) return;
            if (any(__targetMoveDirectionRelativeToCamera == F32.PositiveInfinity)) return;
            //TODO: REMOVE THIS REMOVE THIS REMOVE THIS!!!!!!!
            if (__targetMoveDirectionRelativeToCamera.ToString() == "float3(NaNf, NaNf, NaNf)") return;
            
            //Debug.Log($"TargetMoveDirectionRelativeToCamera: {__targetMoveDirectionRelativeToCamera}");
            
            _references.Motor.Move(deltaTime: Time.unscaledDeltaTime);
            
            _references.Motor.Move(deltaTime: Time.deltaTime);
        }
        
        /// <summary>
        /// Move the character when on walkable ground.
        /// </summary>
        private void GroundedMovement(F32x3 desiredVelocity)
        {
            //Debug.Log("GroundedMovement");
            
            _references.Motor.velocity = lerp(
                _references.Motor.velocity, 
                desiredVelocity,
                1f - exp(-_settings.GroundFriction * Time.deltaTime));
        }

        // /// <summary>
        // /// Move the character when falling or on not-walkable ground.
        // /// </summary>
        // private void NotGroundedMovement(F32x3 desiredVelocity)
        // {
        //     F32x3 __velocity = (F32x3)_references.Motor.velocity;
        //
        //     // If moving into non-walkable ground, limit its contribution.
        //     // Allow movement parallel, but not into it because that may push us up.
        //     if (_references.Motor.isOnGround && dot(desiredVelocity, (F32x3)_references.Motor.groundNormal) < 0.0f)
        //     {
        //         F32x3 __groundNormal = _references.Motor.groundNormal;
        //
        //         F32x3 __planeNormal  = normalize(new F32x3(x: __groundNormal.x, y: 0, z: __groundNormal.y));
        //
        //         desiredVelocity = desiredVelocity.ProjectedOnPlane(planeNormal: __planeNormal);
        //     }
        //     
        //     F32x3 __flatVelocity = new(x: __velocity.x, y: 0,            z: __velocity.z);
        //     F32x3 __verVelocity  = new(x: 0,            y: __velocity.y, z: 0);
        //
        //     // Accelerate horizontal velocity towards desired velocity
        //     F32x3 __horizontalVelocity = __flatVelocity.MoveTowards(
        //         target:  desiredVelocity, 
        //         maxDistanceDelta: (F32)_settings.MaxAcceleration * (F32)_settings.AirControlPrimantissa * Time.deltaTime);
        //
        //     // Update velocity preserving gravity effects (vertical velocity)
        //     __velocity = __horizontalVelocity + __verVelocity;
        //
        //     // Apply gravity
        //     __velocity += (F32x3)_settings.Gravity * Time.deltaTime;
        //
        //     // Apply Air friction (Drag)
        //     __velocity -= __velocity * (F32)_settings.AirFriction * Time.deltaTime;
        //     //__velocity -= clamp(1.0f - ((F32)_settings.AirFriction * Time.deltaTime), 0.0f, 1.0f);
        //
        //     // Update character's velocity
        //     _references.Motor.velocity = __velocity;
        // }
    }
    
    [Serializable]
    public struct MoveSettings 
    {
        [field:Tooltip(tooltip: "The character's maximum speed. (m/s)")]
        [field:SerializeField] public Constant<F32>   MaxSpeed                    { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "Max Acceleration (rate of change of velocity).")]
        [field:SerializeField] public Constant<F32>   MaxAcceleration             { get; [UsedImplicitly] private set; }
        
        //[field:SerializeField] public Constant<F32>   MoveDirectionSmoothingSpeed { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "Setting that affects movement control. Higher values allow faster changes in direction.")]
        [field:SerializeField] public Constant<F32>   GroundFriction              { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "Friction to apply when falling.")]
        [field:SerializeField] public Constant<F32>   AirFriction                 { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "When falling, amount of horizontal movement control available to the character.\n" +
                                "0 = no control, 1 = full control at max acceleration.")]
        [field:SerializeField] public Constant<F32>   AirControlPrimantissa       { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "The character's gravity.")] 
        [field:SerializeField] public Constant<F32x3> Gravity                     { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public Constant<F32>   OrientationSpeed            { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Variable<F32x3> OrientationLookDirection    { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public ScriptableEvent<F32x3> OnMove               { get; [UsedImplicitly] private set; }
        
    }
}