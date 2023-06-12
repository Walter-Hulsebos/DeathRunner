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
        [Tooltip("AnimationClip: Attack Animation, F32: Attack Speed Multiplier")]
        [SerializeField] private ScriptableEvent<AnimationClip, F32> onAttackStarted;
        #if ODIN_INSPECTOR
        [FoldoutGroup(groupName: "Events")]
        #endif
        [SerializeField] private ScriptableEvent<F32> onAttackStopped;
        
        private Bool _hasAlreadyDisabledRootMotion = false;
        
        private F32 _fadeDuration = AnimancerPlayable.DefaultFadeDuration;

        #endregion
        
        #region Methods
        
        private void OnEnable()
        {
            onAttackStarted += OnAttackStartedHandler;
            //onAttackStopped += DisableRootMotion;
            onAttackStopped += SetFadeDuration;

        }
        private void OnDisable()
        {
            onAttackStarted -= OnAttackStartedHandler;
            //onAttackStopped -= DisableRootMotion;
            onAttackStopped -= SetFadeDuration;
        }
        
        private void OnAttackStartedHandler(AnimationClip attackAnimation, F32 attackSpeedMultiplier)
        {
            Debug.Log("OnAttackStartedHandler");
            
            //AnimancerState __state = MyAnimancer.Play(clip: attackAnimation);
            AnimancerState __state = MyAnimancer.Play(clip: attackAnimation, fadeDuration: _fadeDuration);
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
        
        private void SetFadeDuration(F32 fadeDuration)
        {
            Debug.Log("Set Fade Duration!!");
            _fadeDuration = fadeDuration;
        }

        #endregion
    }
}