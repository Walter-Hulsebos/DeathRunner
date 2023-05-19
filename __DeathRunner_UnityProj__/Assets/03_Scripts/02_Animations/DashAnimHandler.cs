using UnityEngine;
using static Unity.Mathematics.math;

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
        [FoldoutGroup(groupName: "Events")]
        #endif
        [SerializeField] private ScriptableEvent<F32x3> onDashEvent;
        
        [SerializeReference] private ITransition dashAnimations;
        
        private MixerState<Vector2> _dashState;

        #endregion
        
        #region Methods

        private void Awake()
        {
            AnimancerState __state = MyAnimancer.States.GetOrCreate(transition: dashAnimations);
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
            
            if (all(x: dashDir == F32x3.zero)) return;

            F32x3 __dashDirection = normalize(x: dashDir);
            F32x3 __dashDirectionNonRelative   = __dashDirection.InverseRelativeTo(relativeToThis: animationReferences.PlayerCamera.transform);

            F32x3 __facingDirection = animationReferences.PlayerTransform.forward;
            F32x3 __facingDirectionNonRelative = __facingDirection.InverseRelativeTo(relativeToThis: animationReferences.PlayerCamera.transform);

            F32x2 __orthogonalDashDirection = normalize(x: new F32x2(
                x: -dot(x: __dashDirectionNonRelative, y: cross(x: __facingDirectionNonRelative, y: up())),
                y: +dot(x: __dashDirectionNonRelative, y: __facingDirectionNonRelative)));

            //animator.SetFloat(id: dash_x, value: __orthogonalDirection.x);
            //animator.SetFloat(id: dash_y, value: __orthogonalDirection.z);
            
            //animator.SetTrigger(id: dash);
            
            _dashState.Parameter = __orthogonalDashDirection;
            _dashState.Speed = 1;
            
            MyAnimancer.Play(state: _dashState);
        }
        
        #endregion
    }
}
