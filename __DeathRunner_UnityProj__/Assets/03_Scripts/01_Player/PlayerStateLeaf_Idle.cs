//System libraries first

//Unity-specific libraries next
using System;
using DeathRunner.Shared;
using GenericScriptableArchitecture;
using HFSM;
using JetBrains.Annotations;
using UnityEngine;

//Third-party libraries next

//Project-specific libraries last
using F32   = System.Single;
using F32x3 = Unity.Mathematics.float3;

namespace DeathRunner.Player
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
            
            #if UNITY_EDITOR
            Debug.Log("State.Idle.Enter");
            #endif
            
            _settings.OnEnterIdle.Invoke();
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            
            #if UNITY_EDITOR
            Debug.Log("State.Idle.Exit");
            #endif
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
            
            _references.Motor.Move(deltaTime: Time.deltaTime);
        }
        
        /// <summary>
        /// Move the character when on walkable ground.
        /// </summary>
        private void GroundedMovement()
        {
            F32x3 __velocity = _references.Motor.velocity;

            // Apply friction
            __velocity -= __velocity * (F32)_settings.GroundFriction * Time.deltaTime;
            //__velocity -= clamp(1.0f - ((F32)_settings.GroundFriction * Time.deltaTime), 0.0f, 1.0f);

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
            __velocity += (F32x3)_settings.Gravity * Time.deltaTime;

            // Apply Air friction (Drag)
            __velocity -= __velocity * (F32)_settings.AirFriction * Time.deltaTime;
            //__velocity -= clamp(1.0f - ((F32)_settings.AirFriction * Time.deltaTime), 0.0f, 1.0f);

            // Update character's velocity
            _references.Motor.velocity = __velocity;
        }
    }
    
    [Serializable]
    public struct IdleSettings 
    {
        [field:Tooltip(tooltip: "Setting that affects movement control. Higher values allow faster changes in direction.")]
        [field:SerializeField] public Constant<F32>   GroundFriction { get; [UsedImplicitly] private set; }
        
        
        //[field:SerializeField] public Constant<Bool>  UseRootMotion  { get; [UsedImplicitly] private set; }

        [field:Tooltip(tooltip: "Friction to apply when falling.")]
        [field:SerializeField] public Constant<F32>   AirFriction    { get; [UsedImplicitly] private set; }

        [field:Tooltip(tooltip: "The character's gravity.")] 
        [field:SerializeField] public Constant<F32x3> Gravity        { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public ScriptableEvent OnEnterIdle    { get; [UsedImplicitly] private set; }
    }
}