//System libraries first
using System;

//Unity-specific libraries next
using UnityEngine;
using static Unity.Mathematics.math;

//Third-party libraries next
using DG.Tweening;
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
    public class PlayerStateLeaf_DashShort : StateLeaf
    {
        #region Variables

        private readonly DashShortSettings _settings;
        private readonly PlayerReferences  _references;
        
        public Bool IsDashing { get; private set; } = false;
        public Bool IsDoneDashing => !IsDashing;
        
        //TODO: Cache all constant settings?

        #endregion

        #region Constructors
        
        public PlayerStateLeaf_DashShort(DashShortSettings settings, PlayerReferences references)
        {
            this._settings   = settings;
            this._references = references;
        }
        
        #endregion
        
        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("State.DashShort.Enter");
            
            // Move the player in the dash direction
            DashMovement(DashDirection);
        }

        protected override void OnExit()    
        {
            base.OnExit();
            
            Debug.Log("State.DashShort.Exit");
        }
        
        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            //Debug.Log("Dash.Update");
        }

        private F32x3 DashDirection
        {
            get
            {
                F32x3 __dashDir = new(x: _references.InputHandler.MoveInput.x, y: 0, z: _references.InputHandler.MoveInput.y);
                
                // If no input is given, dash in the direction the player is facing
                if (all(x: __dashDir == F32x3.zero))
                {
                    __dashDir = (F32x3)_settings.OrientationLookDirection;
                }
                
                // Convert dash direction to be relative to the player camera
                return __dashDir.RelativeTo(relativeToThis: _references.Camera.transform);   
            }
        }
        
        private void DashMovement(F32x3 direction)
        {
            //TODO: Recalculate dash end position every frame?
            
            //NOTE: [Walter] This completely disregards any collisions that might occur during the dash if they're not detected by the initial sweep test. Whether this is good or bad is TBD.
                
            // Calculate dash end position
            Bool __hitsSomethingWhileDashing = _references.Motor.MovementSweepTest(characterPosition: _references.Motor.position, sweepDirection: direction, sweepDistance: (F32)_settings.MaxDistance, out CollisionResult __collisionResult);
            
            F32x3 __displacement = (__hitsSomethingWhileDashing) 
                ? (F32x3)__collisionResult.displacementToHit 
                : (direction * (F32)_settings.MaxDistance);
            
            IsDashing = true;
            _settings.OnDashBegin.Invoke(direction);

            F32 __dashTime = length(__displacement) / (F32)_settings.MaxSpeed;
            
            DOTween.To(
                    getter: () => _references.Motor.position, 
                    setter: pos =>
                    {
                        _references.Motor.interpolation = RigidbodyInterpolation.None;
                        _references.Motor.SetPosition(pos, updateGround: true);
                        _references.Motor.interpolation = RigidbodyInterpolation.Interpolate;
                    },
                    endValue: _references.Motor.position + __displacement, 
                    duration: __dashTime)
                .OnComplete(() =>
                {
                    _settings.OnDashEnd.Invoke();
                    IsDashing = false;
                });
        }
    }
    
    [Serializable]
    public struct DashShortSettings
    {
        [field:Tooltip(tooltip: "The max dash speed (m/s)")]
        [field:SerializeField] public Constant<F32>   MaxSpeed                 { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "The max dash distance (m)")]
        [field:SerializeField] public Constant<F32>   MaxDistance              { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public Constant<U16>   ConsumptionPerDash       { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "The duration between possible dashes (s)")]
        [field:SerializeField] public Constant<F32>   DashCooldown             { get; [UsedImplicitly] private set; }
        
        //[field:SerializeField] public Variable<Bool>  OrientTowardsCursor      { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Variable<F32x3> OrientationLookDirection { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public ScriptableEvent<F32x3> OnDashBegin       { get; [UsedImplicitly] private set; }
        [field:SerializeField] public ScriptableEvent        OnDashEnd         { get; [UsedImplicitly] private set; }
        [field:SerializeField] public ScriptableEvent        OnDashInterrupted { get; [UsedImplicitly] private set; }
    }
}
