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
        [SerializeField] private ScriptableEvent<AnimationClip> onAttackStarted;
        
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
        
        private void OnAttackStartedHandler(AnimationClip attackAnimation)
        {
            Animancer.Play(attackAnimation);
        }

        #endregion
    }
}