 using UnityEngine;

using GenericScriptableArchitecture;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace DeathRunner.Animations
{
    public sealed class IdleAnimHandler : AnimHandler
    {
        #region Variables
        
        #if ODIN_INSPECTOR
        [BoxGroup(group: "AnimationClips", showLabel: false)]
        #endif
        [SerializeField] private AnimationClip idleClip;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Events")]
        #endif
        [SerializeField] private ScriptableEvent onEnterIdle;
        
        #endregion
        
        #region Methods
        
        private void OnEnable()
        {
            onEnterIdle += OnEnterIdleHandler;
        }
        private void OnDisable()
        {
            onEnterIdle -= OnEnterIdleHandler;
        }
        
        private void OnEnterIdleHandler()
        {
            Animancer.Play(idleClip);
        }
        
        #endregion
    }
}