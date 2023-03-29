
using DeathRunner.Inputs;
using EasyCharacterMovement;
using DeathRunner.Movement;
using DG.Tweening;
using UnityEngine;

using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

using ProjectDawn.Mathematics;
using Sirenix.OdinInspector;
using Bool  = System.Boolean;

using static Unity.Mathematics.math;

namespace Game
{
    public class AnimationEventsPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject scytheHitbox;
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] private Orientation _orientation;
        [SerializeField] private Camera playerCamera;

        [SerializeField] private CharacterMotor _motor;
        
#if ODIN_INSPECTOR
        [SuffixLabel("m")]
#endif
        [SerializeField] private F32 dashDistance = 10f; // Distance of dash in meters
        
#if ODIN_INSPECTOR
        [SuffixLabel("m/s")]
#endif
        [SerializeField] private F32 dashSpeed = 15f;

        private bool _isDashing = false;
        
        
         private Rigidbody rb;
         private bool isMoving = false;
         
         
        [SerializeField] private Locomotion _locomotion;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            scytheHitbox.SetActive(false);

        }

        public void DisableInputs()
        {
            _locomotion.enabled = false;
            _orientation.enabled = false;
        }
        
        public void EnableInputs()
        {
            _locomotion.enabled = true;
            _orientation.enabled = true;
        }
        // Start is called before the first frame update
        public void EnableHitbox()
        {
            scytheHitbox.SetActive(true);
            //TODO disable player movement when state machine is there
           
        }

        // Update is called once per frame
        public void DisableHitbox()
        {
            scytheHitbox.SetActive(false);
        }
        
        public void StartMoving()
        {
           // if (isMoving){
           F32x3 __dashDir = new F32x3(x: _inputHandler.MoveInput.x, y: 0, z: _inputHandler.MoveInput.y);
                __dashDir = _orientation.LookDirection;
                // Convert dash direction to be relative to the player camera
                F32x3 __relativeDashDir = math2.RelativeTo(__dashDir, relativeToThis: playerCamera.transform);

                // Move the player in the dash direction
                //DashMovement(__relativeDashDir).Forget();
                DashMovement(__relativeDashDir);
                __relativeDashDir = F32x3.zero;
                //  isMoving = true;
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
                    Debug.Log(message: "Dash - End");
                    _isDashing = false;
                });
        }
        public void StopMoving()
        {
            isMoving = false;
        }

        private void Update()
        {
  
           }
        }
    }

