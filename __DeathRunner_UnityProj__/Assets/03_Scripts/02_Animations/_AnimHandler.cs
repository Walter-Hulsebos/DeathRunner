//First 

using Animancer;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace DeathRunner.Animations
{
    public abstract class AnimHandler : MonoBehaviour
    {
        #region Variables
        
        [FormerlySerializedAs("animationController")]
        [BoxGroup(group: "References", showLabel: false)]
        [SerializeField] protected PlayerAnimationReferences animationReferences;

        public AnimancerComponent Animancer       => animationReferences.Animancer;
        public Transform          PlayerTransform => animationReferences.PlayerTransform;
        public Camera             PlayerCamera    => animationReferences.PlayerCamera;

        #endregion
        
        #region Methods
        
        #if UNITY_EDITOR

        protected virtual void Reset()
        {
            FindPlayerAnimationController();
        }
        
        protected virtual void OnValidate()
        {
            if (animationReferences == null)
            {
                FindPlayerAnimationController();
            }
        }
        
        private void FindPlayerAnimationController()
        {
            animationReferences = GetComponentInParent<PlayerAnimationReferences>();
        }
        
        #endif
        
        #endregion
    }
}
