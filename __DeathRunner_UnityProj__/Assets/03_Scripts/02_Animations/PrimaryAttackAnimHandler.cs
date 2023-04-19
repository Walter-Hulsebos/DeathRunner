using Animancer;
using UnityEngine;

using GenericScriptableArchitecture;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using F32 = System.Single;

namespace DeathRunner.Animations
{
    public sealed class PrimaryAttackAnimHandler : AnimHandler
    {
        #region Variables

        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private ScriptableEvent<AnimationClip, F32> onAttackStarted;
        
        #endregion
        
        #region Methods
        
        private void OnEnable()
        {
            onAttackStarted += OnAttackStartedHandler;
        }
        private void OnDisable()
        {
            onAttackStarted -= OnAttackStartedHandler;
        }
        
        private void OnAttackStartedHandler(AnimationClip attackAnimation, F32 attackSpeedMultiplier)
        {
            AnimancerState __state = Animancer.Play(attackAnimation);
            __state.Speed = attackSpeedMultiplier;
        }

        #endregion
    }
}