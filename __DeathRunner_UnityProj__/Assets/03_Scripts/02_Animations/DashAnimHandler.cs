using UnityEngine;

using Drawing;
using GenericScriptableArchitecture;
using ProjectDawn.Mathematics;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using static Unity.Mathematics.math;

using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

namespace DeathRunner.Animations
{
    public sealed class DashAnimHandler : AnimHandler
    {
        #region Variables
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private ScriptableEvent<F32x3> onDashEvent;
        
        private F32x2 _orthogonalDashDirection;

        #endregion
        
        #region Methods

        private void OnEnable()
        {
            onDashEvent += OnDashHandler;
        }
        
        private void OnDisable()
        {
            onDashEvent -= OnDashHandler;
        }
        
        private void OnDashHandler(F32x3 dashDir)
        {
            Debug.Log(message: $"DashDir: {dashDir}");
            
            if (all(dashDir == F32x3.zero))
            {
                _orthogonalDashDirection = F32x2.zero;
                return;
            }

            F32x3 __dashDirection = normalize(dashDir);
            F32x3 __dashDirectionNonRelative   = __dashDirection.InverseRelativeTo(animationReferences.PlayerCamera.transform);

            F32x3 __facingDirection = animationReferences.PlayerTransform.forward;
            F32x3 __facingDirectionNonRelative = __facingDirection.InverseRelativeTo(animationReferences.PlayerCamera.transform);

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
