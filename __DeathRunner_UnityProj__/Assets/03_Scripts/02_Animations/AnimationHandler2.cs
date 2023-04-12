using Drawing;
using GenericScriptableArchitecture;
using JetBrains.Annotations;
using ProjectDawn.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

using F32x3 = Unity.Mathematics.float3;

using I32   = System.Int32;

namespace DeathRunner.Animations
{
    public sealed class AnimationHandler2 : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Transform characterTransform;
        
        [SerializeField] private Camera playerCamera;

        [SerializeField] private Animator animator;
        
        [SerializeField] private ScriptableEvent<F32x3> onMoveEvent;
        
        [SerializeField] private ScriptableEvent<F32x3> onDashEvent;

        private static readonly I32 move_x = Animator.StringToHash(name: "MoveX");
        private static readonly I32 move_y = Animator.StringToHash(name: "MoveY");
        
        private static readonly I32 attack = Animator.StringToHash(name: "Attack");
        private static readonly I32 dash   = Animator.StringToHash(name: "Dash");
        
        private static readonly I32 dash_x = Animator.StringToHash(name: "DashDirX");
        private static readonly I32 dash_y = Animator.StringToHash(name: "DashDirY");
        

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

        private void OnEnable()
        {
            onMoveEvent += OnMoveHandler;
            onDashEvent += OnDashHandler;
        }
        
        private void OnDisable()
        {
            onMoveEvent -= OnMoveHandler;
            onDashEvent -= OnDashHandler;
        }

        private void OnMoveHandler(F32x3 moveVector)
        {
            //Debug.Log(message: $"MoveVector: {moveVector}");
            
            if (all(moveVector == F32x3.zero))
            {
                animator.SetFloat(id: move_x, value: 0);
                animator.SetFloat(id: move_y, value: 0);
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
                to:   __characterPosition + moveVector,
                color: Color.cyan);

            //Draw the facing vector
            Draw.Arrow(
                from: __characterPosition,
                to:   __characterPosition + __facingDirection,
                color: Color.green);

            animator.SetFloat(id: move_x, value: __orthogonalDirection.x);
            animator.SetFloat(id: move_y, value: __orthogonalDirection.z);
        }
        
        [PublicAPI]
        public void TriggerAttack()
        {
            animator.SetTrigger(id: attack);
        }
        
        [PublicAPI]
        public void OnDashHandler(F32x3 dashDir)
        {
            Debug.Log(message: $"DashDir: {dashDir}");
            
            if (all(dashDir == F32x3.zero))
            {
                animator.SetFloat(id: dash_x, value: 0);
                animator.SetFloat(id: dash_y, value: 0);
                return;
            }

            F32x3 __dashDirection = normalize(dashDir);
            F32x3 __dashDirectionNonRelative   = __dashDirection.InverseRelativeTo(playerCamera.transform);

            F32x3 __facingDirection = characterTransform.forward;
            F32x3 __facingDirectionNonRelative = __facingDirection.InverseRelativeTo(playerCamera.transform);

            F32x3 __orthogonalDirection = normalize(new F32x3(
                x: -dot(__dashDirectionNonRelative, cross(__facingDirectionNonRelative, up())), 
                y: 0f, 
                z: dot(__dashDirectionNonRelative, __facingDirectionNonRelative)));

            animator.SetFloat(id: dash_x, value: __orthogonalDirection.x);
            animator.SetFloat(id: dash_y, value: __orthogonalDirection.z);
            
            animator.SetTrigger(id: dash);
        }

        #endregion
    }
}
