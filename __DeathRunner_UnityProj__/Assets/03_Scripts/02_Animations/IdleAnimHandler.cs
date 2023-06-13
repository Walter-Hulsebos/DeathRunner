 using System;
 using Animancer;
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
        [FoldoutGroup(groupName: "Events")]
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

        private void Start()
        {
            //To ensure starting in idle.
            OnEnterIdleHandler();
        }

        private void OnEnterIdleHandler()
        {
            //TODO: Look into normalized fade mode?
            AnimancerState __state = MyAnimancer.Play(clip: idleClip, fadeDuration: AnimancerPlayable.DefaultFadeDuration);
            //  __state.Stop();
        }
        
        #endregion
    }
}