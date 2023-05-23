using Animancer;
using UnityEngine;

using GenericScriptableArchitecture;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using F32 = System.Single;

using Bool = System.Boolean;

namespace DeathRunner.Animations
{
    public sealed class PrimaryAttackAnimHandler : AnimHandler
    {
        #region Variables

        #if ODIN_INSPECTOR
        [FoldoutGroup(groupName: "Events")]
        #endif
        [SerializeField] private ScriptableEvent<AnimationClip, F32> onAttackStarted;
        #if ODIN_INSPECTOR
        [FoldoutGroup(groupName: "Events")]
        #endif
        [SerializeField] private ScriptableEvent onAttackStopped;
        
        private Bool _hasAlreadyDisabledRootMotion = false;

        #endregion
        
        #region Methods
        
        private void OnEnable()
        {
            onAttackStarted += OnAttackStartedHandler;
            onAttackStopped += DisableRootMotion;
            
        }
        private void OnDisable()
        {
            onAttackStarted -= OnAttackStartedHandler;
            onAttackStopped -= DisableRootMotion;
        }
        
        private void OnAttackStartedHandler(AnimationClip attackAnimation, F32 attackSpeedMultiplier)
        {
            AnimancerState __state = MyAnimancer.Play(clip: attackAnimation);
            __state.Speed = attackSpeedMultiplier;
            
            EnableRootMotion();
        }
        
        private void EnableRootMotion()
        {
            MyAnimancer.Animator.applyRootMotion = true;
            _hasAlreadyDisabledRootMotion = false;
        }
        
        private void DisableRootMotion()
        {
            if(_hasAlreadyDisabledRootMotion) return;
            MyAnimancer.Animator.applyRootMotion = false;

            _hasAlreadyDisabledRootMotion = true;
        }

        #endregion
    }
}