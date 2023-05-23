using UnityEngine;

using GenericScriptableArchitecture;
using UnityEngine.Serialization;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace DeathRunner.Animations
{
    public sealed class DeathAnimHandler : AnimHandler
    {
        #region Variables
        
        #if ODIN_INSPECTOR
        [BoxGroup(group: "AnimationClips", showLabel: false)]
        #endif
        [SerializeField] private AnimationClip deathAnimation;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup(groupName: "Events")]
        #endif
        [SerializeField] private ScriptableEvent onDeath;
        
        #endregion
        
        #region Methods
        
        private void OnEnable()
        {
            onDeath += OnDeathHandler;
        }
        private void OnDisable()
        {
            onDeath -= OnDeathHandler;
        }
        
        private void OnDeathHandler()
        {
            MyAnimancer.Play(clip: deathAnimation);
        }
        
        #endregion
    }
}