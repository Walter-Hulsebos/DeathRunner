//System libraries first
using System;

//Unity-specific libraries next
using UnityEngine;
using static Unity.Mathematics.math;

//Third-party libraries next
using JetBrains.Annotations;
using HFSM;
using GenericScriptableArchitecture;

//Project-specific libraries last
using DeathRunner.Shared;

using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;

namespace DeathRunner.PlayerState
{
    public sealed class PlayerStateLeaf_Idle : StateLeaf
    {
        private readonly IdleSettings     _settings;
        private readonly PlayerReferences _references;

        public PlayerStateLeaf_Idle(IdleSettings settings, PlayerReferences references)
        {
            this._settings   = settings;
            this._references = references;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Idle.Enter");
            
            _settings.OnEnterIdle.Invoke();
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            Debug.Log("Idle.Exit");
        }

        private F32x3 _moveDirectionVelocity;
        protected override void OnLateFixedUpdate()
        {
            base.OnLateFixedUpdate();
            
            //Debug.Log("Idle.LateFixedUpdate");
            
            // Update character’s velocity based on its grounding status
            if (_references.Motor.isGrounded)
            {
                GroundedMovement();
            }
            else
            {
                NotGroundedMovement();
            }
            
            _references.Motor.Move(deltaTime: Commands.DeltaTime);
        }
        
        /// <summary>
        /// Move the character when on walkable ground.
        /// </summary>
        private void GroundedMovement()
        {
            F32x3 __velocity = _references.Motor.velocity;

            // Apply friction
            __velocity -= __velocity * (F32)_settings.GroundFriction * Commands.DeltaTime;
            //__velocity -= clamp(1.0f - ((F32)_settings.GroundFriction * Commands.DeltaTime), 0.0f, 1.0f);

            // Update character's velocity
            _references.Motor.velocity = __velocity;
        }

        /// <summary>
        /// Move the character when falling or on not-walkable ground.
        /// </summary>
        private void NotGroundedMovement()
        {
            F32x3 __velocity = (F32x3)_references.Motor.velocity;

            // Apply gravity
            __velocity += (F32x3)_settings.Gravity * Commands.DeltaTime;

            // Apply Air friction (Drag)
            __velocity -= __velocity * (F32)_settings.AirFriction * Commands.DeltaTime;
            //__velocity -= clamp(1.0f - ((F32)_settings.AirFriction * Commands.DeltaTime), 0.0f, 1.0f);

            // Update character's velocity
            _references.Motor.velocity = __velocity;
        }
    }
    
    [Serializable]
    public struct IdleSettings 
    {
        [field:Tooltip(tooltip: "Setting that affects movement control. Higher values allow faster changes in direction.")]
        [field:SerializeField] public Constant<F32>   GroundFriction { get; [UsedImplicitly] private set; }
        
        [field:Tooltip(tooltip: "Friction to apply when falling.")]
        [field:SerializeField] public Constant<F32>   AirFriction    { get; [UsedImplicitly] private set; }

        [field:Tooltip(tooltip: "The character's gravity.")] 
        [field:SerializeField] public Constant<F32x3> Gravity        { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public ScriptableEvent OnEnterIdle    { get; [UsedImplicitly] private set; }
    }
}