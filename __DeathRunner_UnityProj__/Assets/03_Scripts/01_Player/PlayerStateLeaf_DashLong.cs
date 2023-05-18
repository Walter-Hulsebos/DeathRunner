//System libraries first
using System;

//Unity-specific libraries next
using UnityEngine;
using static Unity.Mathematics.math;

//Third-party libraries next
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
            
            Debug.Log("DashLong.Enter");
            
            // Move the player in the dash direction
            DashMovement(DashDirection);
        }

        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("DashLong.Exit");
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
           
        }
    }
    
    [Serializable]
    public struct DashLongSettings
    {
        [field:Tooltip(tooltip: "The max dash speed (m/s)")]
        [field:SerializeField] public Constant<F32>   MaxSpeed                 { get; [UsedImplicitly] private set; }

        [field:SerializeField] public Constant<U16>   ConsumptionPerSecond     { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "The duration between possible dashes (s)")]
        [field:SerializeField] public Constant<F32>   DashCooldown             { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public Variable<F32x3> OrientationLookDirection { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public ScriptableEvent<F32x3> OnDashBegin       { get; [UsedImplicitly] private set; }
        [field:SerializeField] public ScriptableEvent        OnDashEnd         { get; [UsedImplicitly] private set; }
        [field:SerializeField] public ScriptableEvent        OnDashInterrupted { get; [UsedImplicitly] private set; }
    }
}
