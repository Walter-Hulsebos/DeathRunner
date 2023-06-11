//System libraries first
using System;
using Cysharp.Threading.Tasks;

//Unity-specific libraries next
using UnityEngine;
using static Unity.Mathematics.math;

//Third-party libraries next
using EasyCharacterMovement;
using GenericScriptableArchitecture;
using HFSM;
using JetBrains.Annotations;
using ProjectDawn.Mathematics;


using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;

using U16   = System.UInt16;

using Bool  = System.Boolean;

namespace DeathRunner.Player
{
    public sealed class PlayerStateLeaf_DashShort2 : StateLeaf
    {
        #region Variables

        private readonly DashShortSettings2 _settings;
        private readonly PlayerReferences  _references;
        
        private Bool _wannaDash = false;
        
        public Bool IsDashing { get; private set; } = false;
        public Bool IsDoneDashing => !IsDashing;

        #endregion

        #region Constructors
        
        public PlayerStateLeaf_DashShort2(DashShortSettings2 settings, PlayerReferences references)
        {
            this._settings   = settings;
            this._references = references;
        }
        
        #endregion
        
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("State.DashShort.Enter");
            
            _wannaDash = true;
        }

        protected override void OnExit()    
        {
            base.OnExit();
            
            Debug.Log("State.DashShort.Exit");
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (!_wannaDash) return;
            _wannaDash = false;
                
            Dash(direction: DashDirection).Forget();
        }

        private F32x3 DashDirection
        {
            get
            {
                F32x3 __dashDir = _references.InputHandler.MoveInputFlat;

                // If no input is given, dash in the direction the player is facing
                if (all(x: __dashDir == F32x3.zero))
                {
                    //__dashDir = (F32x3)_settings.OrientationLookDirection;
                    return (F32x3)_references.Motor.transform.forward;
                }
                else
                {
                    __dashDir = normalizesafe(__dashDir);
                    // Convert dash direction to be relative to the player camera
                    return __dashDir.RelativeTo(relativeToThis: _references.Camera.transform);  
                }
            }
        }
        
        private async UniTaskVoid Dash(F32x3 direction)
        {
            F32x3 __positionAtDashStart = _references.Motor.position;
            F32 __distance = _settings.Distance.Value;
            F32 __time     = _settings.Duration.Value;
            F32 __speed    = __distance / __time;
            
            F32x3 __dashEndPosition = __positionAtDashStart + (direction * __distance);
            
            //Move back the begin position -0.5 units in the direction we want to dash.
            //Check if we hit something within a 0.5 unit sweep test, if so, set the end position to the current motor position.
            //This is to avoid the player phasing through walls.
            Bool __cantDash = _references.Motor.MovementSweepTest(
                characterPosition: __positionAtDashStart - (direction * 0.5f), 
                sweepDirection:    direction, 
                sweepDistance:     0.75f, 
                out CollisionResult __canDashCollisionResult);
            if (__cantDash)
            {
                __dashEndPosition = _references.Motor.position;
            }

            //Move the player up a little to avoid ground collision issues.
            _references.Motor.interpolation = RigidbodyInterpolation.None;
            _references.Motor.SetPosition(_references.Motor.position + up() * 0.05f, updateGround: false);
            _references.Motor.interpolation = RigidbodyInterpolation.Interpolate;

            _settings.OnDashBegin.Invoke(direction);
            Debug.Log("Short Dash Begin, time: " + Time.time);

            while (length(_references.Motor.position - __dashEndPosition) >= EPSILON)
            {
                Bool __isInterrupted = _references.Motor.MovementSweepTest(
                    characterPosition: _references.Motor.position, 
                    sweepDirection:    direction, 
                    sweepDistance:     __distance, 
                    out CollisionResult __sweepCollisionResult);

                if (__isInterrupted)
                {
                    __distance        = length(__sweepCollisionResult.displacementToHit);
                    __dashEndPosition = _references.Motor.position + (direction * __distance);
                    
                    Debug.Log($"Short Dash Interrupted by {__sweepCollisionResult.collider.name}");
                }
                
                _references.Motor.interpolation = RigidbodyInterpolation.None;
                _references.Motor.SetPosition(_references.Motor.position + direction * (__speed * Time.deltaTime), updateGround: false);
                _references.Motor.interpolation = RigidbodyInterpolation.Interpolate;

                await UniTask.WaitForFixedUpdate();
                Debug.Log("Short Dash Update");
            }
            
            Debug.Log("Short Dash End, time: " + Time.time);
            _settings.OnDashEnd.Invoke();
        }
    }
    
    [Serializable]
    public struct DashShortSettings2
    {
        [field:SerializeField] public Constant<F32>   Duration                 { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Constant<F32>   Distance                 { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public Constant<U16>   ConsumptionPerDash       { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "The duration between possible dashes (s)")]
        [field:SerializeField] public Constant<F32>   DashCooldown             { get; [UsedImplicitly] private set; }

        [field:SerializeField] public Variable<F32x3> OrientationLookDirection { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public ScriptableEvent<F32x3> OnDashBegin       { get; [UsedImplicitly] private set; }
        [field:SerializeField] public ScriptableEvent        OnDashEnd         { get; [UsedImplicitly] private set; }
        [field:SerializeField] public ScriptableEvent        OnDashInterrupted { get; [UsedImplicitly] private set; }
    }
}
