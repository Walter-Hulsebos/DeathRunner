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

using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

namespace DeathRunner.Animations
{
    public sealed class MoveAnimHandler : AnimHandler
    {
        #region Variables
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private ScriptableEvent<F32x3> onMoveEvent;

        [SerializeReference] private ITransition moveAnimations;
        
        private MixerState<Vector2> _moveState;
        
        private F32x2 _orthogonalMoveDirection;

        #endregion
        
        #region Methods

        private void Awake()
        {
            // Move is an ITransition which doesn't have a Parameter property for us to control in Update.
            // So we need to create its state, type cast it to MixerState<F32x2>, and store it in a field.
            // Then we will be able to control that field's Parameter in Update.
            //AnimancerState __state = Animancer.States.GetOrCreate(animationReferences.Move);
            AnimancerState __state = Animancer.States.GetOrCreate(moveAnimations);
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
        private void OnMoveHandler(F32x3 targetMoveVector)
        {
            //Debug.Log($"Move vector: {targetMoveVector}");
            
            if (all(targetMoveVector == F32x3.zero))
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
        
            // Set the move direction's length to that of the target.
            //__moveDirection = normalize(__moveDirection) * length(__targetMoveDir);
        
            //        Debug.Log(message: $"__moveDirection: {__moveDirection}");

            F32x3 __moveDirection = normalize(__moveVector);
            F32x3 __moveDirectionNonRelative   = __moveDirection.InverseRelativeTo(PlayerCamera.transform);

            F32x3 __facingDirection = PlayerTransform.forward;
            F32x3 __facingDirectionNonRelative = __facingDirection.InverseRelativeTo(PlayerCamera.transform);

            _orthogonalMoveDirection = normalize(new F32x2(
                x: -dot(__moveDirectionNonRelative, cross(__facingDirectionNonRelative, up())),
                y: +dot(__moveDirectionNonRelative, __facingDirectionNonRelative)));

            #if UNITY_EDITOR
            F32x3 __characterPosition = PlayerTransform.position;
            //Draw the moveVector 
            Draw.Arrow(
                from: __characterPosition,
                to:   __characterPosition + targetMoveVector,
                color: Color.cyan);

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
            
            Animancer.Play(_moveState);
        }
        
        #endregion
    }
}
