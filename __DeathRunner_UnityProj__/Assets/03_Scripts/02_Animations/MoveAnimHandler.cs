using UnityEngine;
using static Unity.Mathematics.math;

using Drawing;
using GenericScriptableArchitecture;
using ProjectDawn.Mathematics;
using Animancer;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using DeathRunner.Utils;
using DeathRunner.Shared;
using static DeathRunner.Utils.MathExtensions;

using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

using Rotor = Unity.Mathematics.quaternion;

namespace DeathRunner.Animations
{
    public sealed class MoveAnimHandler : AnimHandler
    {
        #region Variables
        
        #if ODIN_INSPECTOR
        [FoldoutGroup(groupName: "Events")]
        #endif
        [SerializeField] private ScriptableEvent<F32x3> onMoveEvent;

        [SerializeField] private F32 smoothTurnInSeconds = 0.2f;
        [SerializeField, HideInInspector] private F32 deltaRadiansPerSecond;
        
        [SerializeReference] private ITransition moveAnimations;
        
        
        private MixerState<Vector2> _moveState;
        
        private F32x2 _orthogonalMoveDirection;

        #endregion
        
        #region Methods

        #if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            
            deltaRadiansPerSecond = (TAU_F32 / smoothTurnInSeconds);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            
            deltaRadiansPerSecond = (TAU_F32 / smoothTurnInSeconds);
        }
        #endif

        private void Awake()
        {
            // Move is an ITransition which doesn't have a Parameter property for us to control in Update.
            // So we need to create its state, type cast it to MixerState<F32x2>, and store it in a field.
            // Then we will be able to control that field's Parameter in Update.
            //AnimancerState __state = Animancer.States.GetOrCreate(animationReferences.Move);
            AnimancerState __state = MyAnimancer.States.GetOrCreate(transition: moveAnimations);
            _moveState = (MixerState<Vector2>)__state;
        }

        private void OnEnable()
        {
            onMoveEvent += OnMoveHandler;
        }
        
        private void OnDisable()
        {
            onMoveEvent -= OnMoveHandler;
        }

        private F32x3 _targetMoveVectorLastFrame;
        private F32x3 _moveVectorLastFrame;
        private F32x3 _moveVectorVelocity;

        private Rotor __moveVectorRotationLastFrame = Rotor.identity;

        private Rotor _derivative;

        [ContextMenu(itemName: "LogAngles")]
        private void LogAngles()
        {
            //Radians to float2
            const F32 RATIO = (1f / 16f);
            
            const F32 ANGLE01 = 0f;
            F32x2 __angle01Vec = new(x: cos(x: ANGLE01), y: sin(x: ANGLE01));
            const F32 ANGLE02 = (1 * RATIO) * TAU_F32;
            F32x2 __angle02Vec = new(x: cos(x: ANGLE02), y: sin(x: ANGLE02));
            const F32 ANGLE03 = (2 * RATIO) * TAU_F32;
            F32x2 __angle03Vec = new(x: cos(x: ANGLE03), y: sin(x: ANGLE03));
            const F32 ANGLE04 = (3 * RATIO) * TAU_F32;
            F32x2 __angle04Vec = new(x: cos(x: ANGLE04), y: sin(x: ANGLE04));
            const F32 ANGLE05 = (4 * RATIO) * TAU_F32;
            F32x2 __angle05Vec = new(x: cos(x: ANGLE05), y: sin(x: ANGLE05));
            const F32 ANGLE06 = (5 * RATIO) * TAU_F32;
            F32x2 __angle06Vec = new(x: cos(x: ANGLE06), y: sin(x: ANGLE06));
            const F32 ANGLE07 = (6 * RATIO) * TAU_F32;
            F32x2 __angle07Vec = new(x: cos(x: ANGLE07), y: sin(x: ANGLE07));
            const F32 ANGLE08 = (7 * RATIO) * TAU_F32;
            F32x2 __angle08Vec = new(x: cos(x: ANGLE08), y: sin(x: ANGLE08));
            const F32 ANGLE09 = (8 * RATIO) * TAU_F32;
            F32x2 __angle09Vec = new(x: cos(x: ANGLE09), y: sin(x: ANGLE09));
            const F32 ANGLE10 = (9 * RATIO) * TAU_F32;
            F32x2 __angle10Vec = new(x: cos(x: ANGLE10), y: sin(x: ANGLE10));
            const F32 ANGLE11 = (10 * RATIO) * TAU_F32;
            F32x2 __angle11Vec = new(x: cos(x: ANGLE11), y: sin(x: ANGLE11));
            const F32 ANGLE12 = (11 * RATIO) * TAU_F32;
            F32x2 __angle12Vec = new(x: cos(x: ANGLE12), y: sin(x: ANGLE12));
            const F32 ANGLE13 = (12 * RATIO) * TAU_F32;
            F32x2 __angle13Vec = new(x: cos(x: ANGLE13), y: sin(x: ANGLE13));
            const F32 ANGLE14 = (13 * RATIO) * TAU_F32;
            F32x2 __angle14Vec = new(x: cos(x: ANGLE14), y: sin(x: ANGLE14));
            const F32 ANGLE15 = (14 * RATIO) * TAU_F32;
            F32x2 __angle15Vec = new(x: cos(x: ANGLE15), y: sin(x: ANGLE15));
            const F32 ANGLE16 = (15 * RATIO) * TAU_F32;
            F32x2 __angle16Vec = new(x: cos(x: ANGLE16), y: sin(x: ANGLE16));
            
            Debug.Log(message: $"Angle 01: {__angle01Vec}");
            Debug.Log(message: $"Angle 02: {__angle02Vec}");
            Debug.Log(message: $"Angle 03: {__angle03Vec}");
            Debug.Log(message: $"Angle 04: {__angle04Vec}");
            Debug.Log(message: $"Angle 05: {__angle05Vec}");
            Debug.Log(message: $"Angle 06: {__angle06Vec}");
            Debug.Log(message: $"Angle 07: {__angle07Vec}");
            Debug.Log(message: $"Angle 08: {__angle08Vec}");
            Debug.Log(message: $"Angle 09: {__angle09Vec}");
            Debug.Log(message: $"Angle 10: {__angle10Vec}");
            Debug.Log(message: $"Angle 11: {__angle11Vec}");
            Debug.Log(message: $"Angle 12: {__angle12Vec}");
            Debug.Log(message: $"Angle 13: {__angle13Vec}");
            Debug.Log(message: $"Angle 14: {__angle14Vec}");
            Debug.Log(message: $"Angle 15: {__angle15Vec}");
            Debug.Log(message: $"Angle 16: {__angle16Vec}");

            //Draw the vectors
            F32x3 __origin = (F32x3)transform.position;
            
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle01Vec.x, y: 0, z: __angle01Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle02Vec.x, y: 0, z: __angle02Vec.y) * 5, color: Color.red, duration: 10); 
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle03Vec.x, y: 0, z: __angle03Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle04Vec.x, y: 0, z: __angle04Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle05Vec.x, y: 0, z: __angle05Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle06Vec.x, y: 0, z: __angle06Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle07Vec.x, y: 0, z: __angle07Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle08Vec.x, y: 0, z: __angle08Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle09Vec.x, y: 0, z: __angle09Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle10Vec.x, y: 0, z: __angle10Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle11Vec.x, y: 0, z: __angle11Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle12Vec.x, y: 0, z: __angle12Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle13Vec.x, y: 0, z: __angle13Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle14Vec.x, y: 0, z: __angle14Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle15Vec.x, y: 0, z: __angle15Vec.y) * 5, color: Color.red, duration: 10);
            Debug.DrawRay(start: __origin, dir: new F32x3(x: __angle16Vec.x, y: 0, z: __angle16Vec.y) * 5, color: Color.red, duration: 10);
            
        }
        
        private void OnMoveHandler(F32x3 targetMoveVector)
        {
            //Debug.Log($"Move vector: {targetMoveVector}");
            
            if (all(x: targetMoveVector == F32x3.zero))
            {
                _orthogonalMoveDirection = F32x2.zero;
                return;
            }
            
            //Smoothly interpolate the move direction
            F32x3 __moveVector = _moveVectorLastFrame.SmoothDamp(
                target: targetMoveVector,
                currentVelocity: ref _moveVectorVelocity,
                deltaTime: Commands.DeltaTime,
                smoothTime: 0.2f,
                maxSpeed: 1000);
            _moveVectorLastFrame = __moveVector;
            F32x3 __moveDirection = normalize(x: __moveVector);
            
            //TODO: Walter, the speed of this slerp will vary based on the distance between the two quaternions.
            //      I think you want to use a constant speed, so you'll need to calculate the time based on the distance.
            //TODO: Look Into RotateTowards
            
            // Rotor __targetRotation = Rotor.LookRotationSafe(forward: normalize(targetMoveVector), up: up());
            //
            // F32 __deltaRadiansPerFrame = deltaRadiansPerSecond * Time.deltaTime;
            // Rotor __moveVectorRotation = __moveVectorRotationLastFrame.RotateTowards(
            //     target: __targetRotation,
            //     maxRadiansDelta: __deltaRadiansPerFrame);
            
            // Rotor __moveVectorRotation = __moveVectorRotationLastFrame
            //     .SmoothDamp(
            //         target: __targetRotation, 
            //         derivative: ref _derivative,
            //         smoothTime: 0.05f);
            //__moveVectorRotationLastFrame = __moveVectorRotation;
            //F32x3 __moveDirection = mul(__moveVectorRotation, new F32x3(x: 0, y: 0, z: 1));

            //Create move vector from rotation
            //F32x3 __moveVector = mul(__moveVectorRotation, new F32x3(x: 0, y: 0, z: 1));
            //_moveVectorLastFrame = __moveVector;
        
            // Set the move direction's length to that of the target.
            //__moveDirection = normalize(__moveDirection) * length(__targetMoveDir);
        
            //        Debug.Log(message: $"__moveDirection: {__moveDirection}");

            
            F32x3 __moveDirectionNonRelative   = __moveDirection.InverseRelativeTo(relativeToThis: MyPlayerCamera.transform);

            F32x3 __facingDirection = MyPlayerTransform.forward;
            F32x3 __facingDirectionNonRelative = __facingDirection.InverseRelativeTo(relativeToThis: MyPlayerCamera.transform);

            _orthogonalMoveDirection = normalize(x: new F32x2(
                x: -dot(x: __moveDirectionNonRelative, y: cross(x: __facingDirectionNonRelative, y: up())),
                y: +dot(x: __moveDirectionNonRelative, y: __facingDirectionNonRelative)));

            #if UNITY_EDITOR
            F32x3 __characterPosition = MyPlayerTransform.position;
            //Draw the moveVector 
            Draw.Arrow(
                from: __characterPosition,
                to:   __characterPosition + targetMoveVector,
                color: Color.cyan);
            
            //Draw the move direction
            Draw.Arrow(
                from: __characterPosition,
                to:   __characterPosition + __moveDirection,
                color: Color.red);

            //Draw the facing vector
            Draw.Arrow(
                from: __characterPosition,
                to:   __characterPosition + __facingDirection,
                color: Color.green);
            #endif
            
            //TODO: Set the animator parameters
            
            // The movement direction is in world space, so we need to convert it to the bot's local space to be
            // appropriate for its current rotation. We do this by using dot-products to determine how much of that
            // direction lies along each axis. This would be unnecessary if we did not rotate at all.
            _moveState.Parameter = _orthogonalMoveDirection;
            _moveState.Speed = 1;
            
            MyAnimancer.Play(state: _moveState);
        }
        
        #endregion
    }
}
