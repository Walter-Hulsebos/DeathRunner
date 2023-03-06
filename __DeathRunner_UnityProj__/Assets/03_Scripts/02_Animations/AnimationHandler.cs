using UnityEngine;
using static Unity.Mathematics.math;

using JetBrains.Annotations;

using Drawing;
using ProjectDawn.Mathematics;

using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

using I32   = System.Int32;

namespace Game.Movement
{
    public sealed class AnimationHandler : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Transform characterTransform;
        
        [SerializeField] private Camera playerCamera;

        [SerializeField, HideInInspector] private Animator animator;
        
        private static readonly I32 right   = Animator.StringToHash(name: "MoveX");
        private static readonly I32 forward = Animator.StringToHash(name: "MoveY");
        
        private static readonly I32 attack  = Animator.StringToHash(name: "Attack");

        #endregion

        #region Methods

        #if UNITY_EDITOR
        
        private void Reset()
        {
            FindAnimatorReference();
            
            FindCharacterTransform();
            
            FindPlayerCamera();
        }
        
        private void OnValidate()
        {
            if (animator == null)
            {
                FindAnimatorReference();
            }
            
            if (characterTransform == null)
            {
                FindCharacterTransform();
            }
            
            if (playerCamera == null)
            {
                FindPlayerCamera();
            }
        }
        
        private void FindAnimatorReference()
        {
            if (!TryGetComponent(out animator))
            {
                Debug.LogWarning(message: $"No Animator component found on {name}!", context: this);
            }
        }

        private void FindCharacterTransform()
        {
            characterTransform = transform.parent;
        }
        
        private void FindPlayerCamera()
        {
            playerCamera = Camera.main;
        }
        #endif

        [PublicAPI]
        public void SetMoveVector(F32x3 moveVector)
        {
            if (all(moveVector == F32x3.zero))
            {
                animator.SetFloat(id: right,   value: 0);
                animator.SetFloat(id: forward, value: 0);
                return;
            }

            F32x3 __moveDirection = normalize(moveVector);
            F32x3 __moveDirectionNonRelative   = __moveDirection.InverseRelativeTo(playerCamera.transform);

            F32x3 __facingDirection = characterTransform.forward;
            F32x3 __facingDirectionNonRelative = __facingDirection.InverseRelativeTo(playerCamera.transform);

            F32x3 __orthogonalDirection = normalize(new F32x3(
                x: -dot(__moveDirectionNonRelative, cross(__facingDirectionNonRelative, up())), 
                y: 0f, 
                z: dot(__moveDirectionNonRelative, __facingDirectionNonRelative)));

            F32x3 __characterPosition = characterTransform.position;
            //Draw the moveVector 
            Draw.Arrow(
                from: __characterPosition,
                to: __characterPosition + moveVector,
                color: Color.cyan);

            //Draw the facing vector
            Draw.Arrow(
                from: __characterPosition,
                to: __characterPosition + __facingDirection,
                color: Color.green);

            animator.SetFloat(id: right,   value: __orthogonalDirection.x);
            animator.SetFloat(id: forward, value: __orthogonalDirection.z);
        }
        
        [PublicAPI]
        public void TriggerAttack()
        {
            animator.SetTrigger(id: attack);
        }

        #endregion
    }
}
