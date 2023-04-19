using UnityEngine;

using JetBrains.Annotations;

using Animancer;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace DeathRunner.Animations
{
    public sealed class PlayerAnimationReferences : MonoBehaviour
    {
        #region Variables
        
        #if ODIN_INSPECTOR
        [field:FoldoutGroup("References")]
        #endif
        [field:SerializeField] public AnimancerComponent Animancer { get; [UsedImplicitly] private set; }

        #if ODIN_INSPECTOR
        [field:FoldoutGroup("References")]
        #endif
        [field:SerializeField] public Transform PlayerTransform { get; [UsedImplicitly] private set; }
      
        #if ODIN_INSPECTOR
        [field:FoldoutGroup("References")]
        #endif
        [field:SerializeField] public Camera PlayerCamera { get; [UsedImplicitly] private set; }
        
        // /// <summary>
        // /// The [<see cref="SerializeReference"/>] attribute allows any <see cref="ITransition"/> to be assigned to
        // /// this field. The <see href="https://kybernetik.com.au/animancer/docs/manual/other/polymorphic">
        // /// Polymorphic</see> page explains this system in more detail.
        // /// </summary>
        // /// <remarks>
        // /// The <see href="https://kybernetik.com.au/animancer/docs/examples/fine-control/speed-and-time">
        // /// Speed And Time</see> example uses a <see cref="ClipTransition"/> to play a single animation.
        // /// <para></para>
        // /// The <see href="https://kybernetik.com.au/animancer/docs/examples/locomotion/directional-blending">
        // /// Directional Blending</see> example uses a <see cref="MixerTransition2D"/> to blend between various movement
        // /// animations.
        // /// </remarks>
        // [field:SerializeReference] public ITransition Move { get; [UsedImplicitly] private set; }

        #endregion
        
        #region Methods
        
        #if UNITY_EDITOR

        private void Reset()
        {
            FindAnimancerReference();

            FindPlayerTransform();
            
            FindPlayerCamera();
        }
        
        private void OnValidate()
        {
            if (Animancer == null)
            {
                FindAnimancerReference();
            }

            if (PlayerTransform == null)
            {
                FindPlayerTransform();
            }
            
            if (PlayerCamera == null)
            {
                FindPlayerCamera();
            }
        }
        
        private void FindAnimancerReference()
        {
            Animancer = GetComponentInChildren<AnimancerComponent>();
        }

        private void FindPlayerTransform()
        {
            //PlayerTransform = transform.parent.transform;
            PlayerTransform = transform;
        }
        
        private void FindPlayerCamera()
        {
            PlayerCamera = Camera.main;
        }

        #endif
        
        #endregion
    }
}
