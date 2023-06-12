//System libraries first
using System;
using DeathRunner.Shared;
using DG.Tweening;

//Unity-specific libraries next
using UnityEngine;
using static Unity.Mathematics.math;

//Third-party libraries next
using GenericScriptableArchitecture;
using HFSM;
using JetBrains.Annotations;
using ProjectDawn.Geometry3D;
using ProjectDawn.Mathematics;


using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;

using U16   = System.UInt16;

using Bool  = System.Boolean;

using Rotor = Unity.Mathematics.quaternion;

namespace DeathRunner.Player
{
    public class PlayerStateLeaf_DashLong : StateLeaf
    {
        #region Variables

        private readonly DashLongSettings _settings;
        private readonly PlayerReferences _references;

        //TODO: Cache all constant settings?

        #endregion

        #region Constructors
        
        public PlayerStateLeaf_DashLong(DashLongSettings settings, PlayerReferences references)
        {
            this._settings   = settings;
            this._references = references;
        }
        
        #endregion
        
        protected override void OnEnter()
        {
            base.OnEnter();
            
            #if UNITY_EDITOR
            Debug.Log("State.DashLong.Enter");
            #endif
            
            _settings.OnLongDashEnter.Invoke();
        }

        protected override void OnExit()
        {
            base.OnExit();
            
            #if UNITY_EDITOR
            Debug.Log("State.DashLong.Exit");
            #endif
            
            _settings.OnLongDashExit.Invoke();
        }

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
                    1f - exp(-_settings.FrictionGround * Time.unscaledDeltaTime));
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
                    airFriction:           _settings.FrictionAir.Value, 
                    gravity:               _settings.Gravity.Value);
            }

            if (all(__targetMoveDirectionRelativeToCamera == F32x3.zero)) return;
            if (any(__targetMoveDirectionRelativeToCamera == F32.NaN)) return;
            if (any(__targetMoveDirectionRelativeToCamera == F32.PositiveInfinity)) return;
            //TODO: REMOVE THIS REMOVE THIS REMOVE THIS!!!!!!!
            if (__targetMoveDirectionRelativeToCamera.ToString() == "float3(NaNf, NaNf, NaNf)") return;
            
            //Debug.Log($"TargetMoveDirectionRelativeToCamera: {__targetMoveDirectionRelativeToCamera}");
            
            _references.Motor.Move(deltaTime: Time.unscaledDeltaTime);
            _settings.OnLongDashMove.Invoke(__targetMoveDirectionRelativeToCamera);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            UpdateLookDirection();

            PlayerHelpers.OrientTowardsDir(references: _references, direction: _settings.OrientationLookDirection.Value, orientationSpeed: _settings.OrientationSpeed);
            //OrientTowardsLookDirection();
        }

        private void UpdateLookDirection()
        {
            F32x3 __lookPositionRelativeToPlayer = PlayerHelpers.LookPositionRelativeToPlayer(_references, useCursor: _settings.OrientTowardsCursor.Value);

            _settings.OrientationLookDirection.Value = normalize(__lookPositionRelativeToPlayer); 

            _references.LookAt.position = (_references.WorldPos + __lookPositionRelativeToPlayer);
        }
    }
    
    [Serializable]
    public struct DashLongSettings
    {
        [field:SerializeField] public Constant<U16>   StaminaConsumptionPerSecond { get; [UsedImplicitly] private set; }

        [field:Tooltip(tooltip: "The character's maximum speed. (m/s)")]
        [field:SerializeField] public Constant<F32>   MaxSpeed                    { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "Max Acceleration (rate of change of velocity).")]
        [field:SerializeField] public Constant<F32>   MaxAcceleration             { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "Setting that affects movement control. Higher values allow faster changes in direction.")]
        [field:SerializeField] public Constant<F32>   FrictionGround              { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "Friction to apply when falling.")]
        [field:SerializeField] public Constant<F32>   FrictionAir                 { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "When falling, amount of horizontal movement control available to the character.\n" +
                                "0 = no control, 1 = full control at max acceleration.")]
        [field:SerializeField] public Constant<F32>   AirControlPrimantissa       { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "The character's gravity.")] 
        [field:SerializeField] public Constant<F32x3> Gravity                     { get; [UsedImplicitly] private set; }
        
        
        [field:SerializeField] public Variable<Bool>  OrientTowardsCursor         { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Variable<F32x3> OrientationLookDirection    { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Constant<F32>   OrientationSpeed            { get; [UsedImplicitly] private set; }
        
        
        [field:SerializeField] public ScriptableEvent        OnLongDashEnter      { get; [UsedImplicitly] private set; }
        [field:SerializeField] public ScriptableEvent<F32x3> OnLongDashMove       { get; [UsedImplicitly] private set; }
        [field:SerializeField] public ScriptableEvent        OnLongDashExit       { get; [UsedImplicitly] private set; }
    }
}
