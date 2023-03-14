using System;
using UnityEngine;
using static Unity.Mathematics.math;

using JetBrains.Annotations;
using Cysharp.Threading.Tasks;
using DeathRunner.Inputs;
using EasyCharacterMovement;
using static ProjectDawn.Mathematics.math2;
using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

using Bool  = System.Boolean;
using Extensions = DeathRunner.Utils.Extensions;

namespace Game.Movement
{
    [RequireComponent(requiredComponent: typeof(InputHandler), requiredComponent2: typeof(CharacterMotor))]
    public class Dash : MonoBehaviour
    {
        // References to other components
        [SerializeField, HideInInspector] private InputHandler   _inputHandler;
        [SerializeField, HideInInspector] private CharacterMotor _motor;
        [SerializeField, HideInInspector] private Locomotion     _locomotion;
        [SerializeField, HideInInspector] private Orientation    _orientation;
        
        [SerializeField] private Camera playerCamera;
        
        // Dash input and direction
        private Bool _dashInput;
        private F32x3 _dashDir;

        // Dash cooldown and status
        private Bool _canDash = true;
        private Bool _isDashing = false;

        private F32x3 _relativeDashDir;
        
        [SerializeField] private F32 dashTime  = 0.1f; // Length of the dash in seconds
        [SerializeField] private F32 dashSpeed = 15f; //Speed of dash

        [UsedImplicitly]
        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTask Update()
        {
            // Check if dash input is given and able to dash
            if (_inputHandler.DashInput && _canDash)
            {
                Debug.Log(message: "dashing");
                _canDash = false;
                _isDashing = true;

                // Get dash direction from input
                _dashDir = new F32x3(x: _inputHandler.MoveInput.x, y: 0, z: _inputHandler.MoveInput.y);

                // If no input is given, dash in the direction the player is facing
                if (all(x: _dashDir == F32x3.zero))
                {
                    _dashDir = _orientation.LookDirection;
                }

                // Convert dash direction to be relative to the player camera
                _relativeDashDir = _dashDir.RelativeTo(relativeToThis: playerCamera.transform);

                // Disable locomotion and orientation while dashing
                _locomotion.enabled = false;
                _orientation.enabled = false;
                
                await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(value: dashTime), ignoreTimeScale: false);

                // Reset dash direction and status, and enable locomotion and orientation
                _dashDir = F32x3.zero;
                _isDashing = false;
                _locomotion.enabled = true;
                _orientation.enabled = true;
                _canDash = true;
                
                // Start the coroutine to end the dash

            }

            if (_isDashing)
            {
                // Move the player in the dash direction
                _motor.Move(newVelocity:_relativeDashDir * dashSpeed);
            }
        }

        #if UNITY_EDITOR
        private void Reset()
        {
            FindCameraReference();
            FindInputHandler();
            FindCharacterMotor();
            FindLocomotion();
            FindOrientation();
        }

        private void OnValidate()
        {
            if(playerCamera == null)
            {
                FindCameraReference();
            }
            
            if(_inputHandler == null)
            {
                FindInputHandler();
            }
            
            if(_motor == null)
            {
                FindCharacterMotor();
            }
            
            if(_locomotion == null)
            {
                FindLocomotion();
            }
            
            if(_orientation == null)
            {
                FindOrientation();
            }
        }

        private void FindCameraReference()
        {
            Camera __mainCamera = Camera.main;
            if(__mainCamera == null)
            {
                Boolean __foundUnTaggedCamera = Extensions.TryFindObjectOfType(out __mainCamera);
                if (__foundUnTaggedCamera)
                {
                    Debug.LogWarning(message: "There was a Camera found in the scene, but it's not tagged as \"MainCamera\", if there is supposed to be one, tag it correctly.", context: this);
                    playerCamera = __mainCamera;
                }
                else
                {
                    Debug.LogError(message: $"No Camera found in the scene. Please add one to the scene. and Reset this {nameof(Orientation)}", context: this);
                }
            }
            else
            {
                playerCamera = __mainCamera;
            }
        }
        
        private void FindInputHandler()
        {
            _inputHandler = GetComponent<InputHandler>();
        }
        
        private void FindCharacterMotor()
        {
            _motor = GetComponent<CharacterMotor>();
        }
        
        private void FindLocomotion()
        {
            _locomotion = GetComponent<Locomotion>();
        }
        
        private void FindOrientation()
        {
            _orientation = GetComponent<Orientation>();
        }
        
        #endif
        
    }
}
