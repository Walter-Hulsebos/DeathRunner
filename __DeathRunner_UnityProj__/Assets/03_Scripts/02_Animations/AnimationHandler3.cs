using Drawing;
using GenericScriptableArchitecture;
using JetBrains.Annotations;
using ProjectDawn.Mathematics;
using Sirenix.OdinInspector;
using UnityEngine;

using static Unity.Mathematics.math;

using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

using I32   = System.Int32;

namespace DeathRunner.Animations
{
    public sealed class AnimationHandler3 : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Transform characterTransform;
        
        [SerializeField] private Camera playerCamera;

        [SerializeField] private Animator animator;
        
        [FoldoutGroup("Events")]
        [SerializeField] private ScriptableEvent        onEnterIdle;
        [FoldoutGroup("Events")]
        [SerializeField] private ScriptableEvent<F32x3> onMoveEvent;
        [FoldoutGroup("Events")]
        [SerializeField] private ScriptableEvent<F32x3> onDashEvent;

        private F32x2 _orthogonalMoveDirection;
        private F32x2 _orthogonalDashDirection;
        

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
            onEnterIdle += OnEnterIdleHandler;
            onMoveEvent += OnMoveHandler;
            onDashEvent += OnDashHandler;
        }
        private void OnDisable()
        {
            onEnterIdle -= OnEnterIdleHandler;
            onMoveEvent -= OnMoveHandler;
            onDashEvent -= OnDashHandler;
        }
        
        private void OnEnterIdleHandler()
        {
            //TODO: Set the animator parameters
        }

        private void OnMoveHandler(F32x3 moveVector)
        {
            if (all(moveVector == F32x3.zero))
            {
                _orthogonalMoveDirection = F32x2.zero;
                return;
            }

            F32x3 __moveDirection = normalize(moveVector);
            F32x3 __moveDirectionNonRelative   = __moveDirection.InverseRelativeTo(playerCamera.transform);

            F32x3 __facingDirection = characterTransform.forward;
            F32x3 __facingDirectionNonRelative = __facingDirection.InverseRelativeTo(playerCamera.transform);

            _orthogonalMoveDirection = normalize(new F32x2(
                x: -dot(__moveDirectionNonRelative, cross(__facingDirectionNonRelative, up())),
                y: +dot(__moveDirectionNonRelative, __facingDirectionNonRelative)));

            #if UNITY_EDITOR
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
            #endif
            
            //TODO: Set the animator parameters
        }

        [PublicAPI]
        public void OnDashHandler(F32x3 dashDir)
        {
            Debug.Log(message: $"DashDir: {dashDir}");
            
            if (all(dashDir == F32x3.zero))
            {
                _orthogonalDashDirection = F32x2.zero;
                return;
            }

            F32x3 __dashDirection = normalize(dashDir);
            F32x3 __dashDirectionNonRelative   = __dashDirection.InverseRelativeTo(playerCamera.transform);

            F32x3 __facingDirection = characterTransform.forward;
            F32x3 __facingDirectionNonRelative = __facingDirection.InverseRelativeTo(playerCamera.transform);

            _orthogonalDashDirection = normalize(new F32x2(
                x: -dot(__dashDirectionNonRelative, cross(__facingDirectionNonRelative, up())),
                y: +dot(__dashDirectionNonRelative, __facingDirectionNonRelative)));

            //animator.SetFloat(id: dash_x, value: __orthogonalDirection.x);
            //animator.SetFloat(id: dash_y, value: __orthogonalDirection.z);
            
            //animator.SetTrigger(id: dash);
            
            //TODO: Set the animator parameters
        }

        #endregion
    }
}
