using System;
using UnityEngine;
using static Unity.Mathematics.math;

using Cysharp.Threading.Tasks;
using EasyCharacterMovement;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using DeathRunner.Inputs;
using DeathRunner.Shared;
using DeathRunner.Utils;
using DG.Tweening;
using ProjectDawn.Mathematics;
using UltEvents;
using UnityEngine.Serialization;
using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

using Bool  = System.Boolean;

namespace DeathRunner.Movement
{
    [RequireComponent(requiredComponent: typeof(InputHandler), requiredComponent2: typeof(CharacterMotor))]
    public class Dash : Module
    {
        // References to other components
        [SerializeField, HideInInspector] private InputHandler   _inputHandler;
        [SerializeField, HideInInspector] private CharacterMotor _motor;
        [SerializeField, HideInInspector] private Locomotion     _locomotion;
        [SerializeField, HideInInspector] private Orientation    _orientation;
        
        [SerializeField] private Camera playerCamera;
        
        #if ODIN_INSPECTOR
        [SuffixLabel("m")]
        #endif
        [SerializeField] private F32 dashDistance = 10f; // Distance of dash in meters
        
        #if ODIN_INSPECTOR
        [SuffixLabel("m/s")]
        #endif
        [SerializeField] private F32 dashSpeed = 15f;

        #if ODIN_INSPECTOR
        [SuffixLabel("s")]
        #endif
        [SerializeField] private F32 dashCooldown = 0.01f; // Cooldown between dashes in seconds
        
        #if UNITY_EDITOR
        #if ODIN_INSPECTOR
        [DisplayAsString]
        #endif
        [SerializeField] private String dashTime;
        #endif
        
        [SerializeField] private UltEvent<F32x3> onDash;
        
        [SerializeField] private UltEvent onEndDash;
        
        [SerializeField] private UltEvent onHitWhileDashing;
        
        // Dash input and direction
        //private Bool  _dashInput;
        //private F32x3 _dashDir;
        //private F32x3 _relativeDashDir;

        // Dash cooldown and status
        //private Bool _canDash = true;
        private Bool _isDashing = false;

        private void Update()
        {
            if (_inputHandler.DashInput && !_isDashing)
            {
                // Get dash direction from input
                F32x3 __dashDir = new F32x3(x: _inputHandler.MoveInput.x, y: 0, z: _inputHandler.MoveInput.y);
                
                // If no input is given, dash in the direction the player is facing
                if (all(x: __dashDir == F32x3.zero))
                {
                    __dashDir = _orientation.LookDirection;
                }
                
                // Convert dash direction to be relative to the player camera
                F32x3 __relativeDashDir = math2.RelativeTo(__dashDir, relativeToThis: playerCamera.transform);

                // Move the player in the dash direction
                //DashMovement(__relativeDashDir).Forget();
                DashMovement(__relativeDashDir);
            }
        }

        public void DashMovement(F32x3 direction)
        {
            //TODO: Recalculate dash end position every frame?
            
            //NOTE: [Walter] This completely disregards any collisions that might occur during the dash if they're not detected by the initial sweep test. Whether this is good or bad is TBD.
                
            // Calculate dash end position
            Bool __hitsSomethingWhileDashing = _motor.MovementSweepTest(characterPosition: _motor.position, sweepDirection: direction, sweepDistance: dashDistance, out CollisionResult __collisionResult);
            
            F32x3 __displacement = (__hitsSomethingWhileDashing) 
                ? (F32x3)__collisionResult.displacementToHit 
                : (direction * dashDistance);
            
            
            Debug.Log(message: "Dash - Begin");
            onDash.Invoke(direction);
            
            _isDashing = true;
            
            // Disable locomotion and orientation while dashing
            _locomotion.enabled  = false;
            _orientation.enabled = false;
            
            F32 __dashTime = length(__displacement) / dashSpeed;
            
            DOTween.To(
                    getter: () => _motor.position, 
                    setter: pos =>
                    {
                        _motor.interpolation = RigidbodyInterpolation.None;
                        _motor.SetPosition(pos, updateGround: true);
                        _motor.interpolation = RigidbodyInterpolation.Interpolate;
                    },
                    endValue: _motor.position + (Vector3)__displacement, 
                    duration: __dashTime)
                .OnComplete(() =>
                {
                    // Re-enable locomotion and orientation
                    _locomotion.enabled  = true;
                    _orientation.enabled = true;
                    
                    Debug.Log(message: "Dash - End");
                    
                    onEndDash.Invoke();
                    
                    _isDashing = false;
                });
        }

        // private async UniTask DashMovement(F32x3 direction)
        // {
        //     //TODO: Recalculate dash end position every frame?
        //         
        //     // Calculate dash end position
        //     Bool __hitsSomethingWhileDashing = _motor.MovementSweepTest(characterPosition: _motor.position, sweepDirection: direction, sweepDistance: dashDistance, out CollisionResult __collisionResult);
        //
        //     F32x3 __displacement = (__hitsSomethingWhileDashing) 
        //         ? (F32x3)__collisionResult.displacementToHit 
        //         : (direction * dashDistance);
        //     
        //     Debug.Log(message: "Dash - Begin");
        //     _isDashing = true;
        //
        //     
        //     // Disable locomotion and orientation while dashing
        //     _locomotion.enabled  = false;
        //     _orientation.enabled = false;
        //         
        //     F32 __dashTime = length(__displacement) / dashSpeed;
        //     while (__dashTime > 0)
        //     {
        //         __dashTime -= Time.deltaTime;
        //         
        //         F32x3 __displacementThisFrame = __displacement * Time.deltaTime;
        //         
        //         F32x3 __positionNextFrame = (F32x3)_motor.position + (__displacement * Time.deltaTime);
        //             
        //         _motor.interpolation = RigidbodyInterpolation.None;
        //         _motor.SetPosition(__positionNextFrame, updateGround: true);
        //         _motor.interpolation = RigidbodyInterpolation.Interpolate;
        //         
        //         await UniTask.Yield();
        //     }
        //         
        //     // Re-enable locomotion and orientation
        //     _locomotion.enabled  = true;
        //     _orientation.enabled = true;
        //     
        //     Debug.Log(message: "Dash - End");
        //     _isDashing = false;
        // }
        
        // [UsedImplicitly]
        // // ReSharper disable once Unity.IncorrectMethodSignature
        // private async UniTask Update()
        // {
        //     // Check if dash input is given and able to dash
        //     if (_inputHandler.DashInput && _canDash)
        //     {
        //         Debug.Log(message: "dashing");
        //         _canDash = false;
        //         _isDashing = true;
        //
        //         // Get dash direction from input
        //         _dashDir = new F32x3(x: _inputHandler.MoveInput.x, y: 0, z: _inputHandler.MoveInput.y);
        //
        //         // If no input is given, dash in the direction the player is facing
        //         if (all(x: _dashDir == F32x3.zero))
        //         {
        //             _dashDir = _orientation.LookDirection;
        //         }
        //
        //         // Convert dash direction to be relative to the player camera
        //         _relativeDashDir = _dashDir.RelativeTo(relativeToThis: playerCamera.transform);
        //
        //         // Disable locomotion and orientation while dashing
        //         _locomotion.enabled = false;
        //         _orientation.enabled = false;
        //         
        //         await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(value: dashTime), ignoreTimeScale: false);
        //
        //         // Reset dash direction and status, and enable locomotion and orientation
        //         _dashDir = F32x3.zero;
        //         _isDashing = false;
        //         _locomotion.enabled = true;
        //         _orientation.enabled = true;
        //         _canDash = true;
        //         
        //         // Start the coroutine to end the dash
        //
        //     }
        //
        //     if (_isDashing)
        //     {
        //         // Move the player in the dash direction
        //         _motor.Move(newVelocity:_relativeDashDir * dashSpeed);
        //     }
        // }

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
            dashTime = $"{dashDistance / dashSpeed:F3}";
            
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
                Boolean __foundUnTaggedCamera = WorldExtensions.TryFindObjectOfType(out __mainCamera);
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
