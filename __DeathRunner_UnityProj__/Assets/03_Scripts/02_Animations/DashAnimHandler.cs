using UnityEngine;
using static Unity.Mathematics.math;

using Drawing;
using GenericScriptableArchitecture;
using ProjectDawn.Mathematics;
using Animancer;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

using Rotor = Unity.Mathematics.quaternion;

namespace DeathRunner.Animations
{
    public sealed class DashAnimHandler : AnimHandler
    {
        #region Variables
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private ScriptableEvent<F32x3> onDashEvent;
        
        [SerializeReference] private ITransition dashAnimations;
        
        private MixerState<Vector2> _dashState;

        #endregion
        
        #region Methods

        private void Awake()
        {
            AnimancerState __state = MyAnimancer.States.GetOrCreate(dashAnimations);
            _dashState = (MixerState<Vector2>)__state;
        }
        
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
            
            if (all(dashDir == F32x3.zero)) return;

            F32x3 __dashDirection = normalize(dashDir);
            F32x3 __dashDirectionNonRelative   = __dashDirection.InverseRelativeTo(animationReferences.PlayerCamera.transform);

            F32x3 __facingDirection = animationReferences.PlayerTransform.forward;
            F32x3 __facingDirectionNonRelative = __facingDirection.InverseRelativeTo(animationReferences.PlayerCamera.transform);

            F32x2 __orthogonalDashDirection = normalize(new F32x2(
                x: -dot(__dashDirectionNonRelative, cross(__facingDirectionNonRelative, up())),
                y: +dot(__dashDirectionNonRelative, __facingDirectionNonRelative)));

            //animator.SetFloat(id: dash_x, value: __orthogonalDirection.x);
            //animator.SetFloat(id: dash_y, value: __orthogonalDirection.z);
            
            //animator.SetTrigger(id: dash);
            
            _dashState.Parameter = __orthogonalDashDirection;
            _dashState.Speed = 1;
            
            MyAnimancer.Play(_dashState);
        }
        
        #endregion
    }
}
