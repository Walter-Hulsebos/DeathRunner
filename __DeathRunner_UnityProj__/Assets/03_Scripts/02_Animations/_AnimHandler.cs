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
        
        [FormerlySerializedAs(oldName: "animationController")]
        [BoxGroup(group: "References", showLabel: false)]
        [SerializeField] protected PlayerAnimationReferences animationReferences;

        public AnimancerComponent MyAnimancer       => animationReferences.Animancer;
        public Transform          MyPlayerTransform => animationReferences.PlayerTransform;
        public Camera             MyPlayerCamera    => animationReferences.PlayerCamera;

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
