//System libraries first
using System;

//Unity-specific libraries next
using UnityEngine;

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
            
            Debug.Log("Idle.LateFixedUpdate");
            
            // Update character’s velocity based on its grounding status
            if (!_references.Motor.isGrounded)
            {
                F32x3 __velocity = _references.Motor.velocity;

                // Apply gravity
                __velocity += (F32x3)_settings.Gravity * Commands.DeltaTime;

                // Apply Air friction (Drag)
                __velocity -= __velocity * (F32)_settings.AirFriction * Commands.DeltaTime;

                // Update character's velocity
                _references.Motor.velocity = __velocity;
            }
            
            _references.Motor.Move(deltaTime: Commands.DeltaTime);
        }
    }
    
    [Serializable]
    public struct IdleSettings 
    {
        [field:Tooltip(tooltip: "Friction to apply when falling.")]
        [field:SerializeField] public Constant<F32>   AirFriction                 { get; [UsedImplicitly] private set; }

        [field:Tooltip(tooltip: "The character's gravity.")] 
        [field:SerializeField] public Constant<F32x3> Gravity                     { get; [UsedImplicitly] private set; }
    }
}