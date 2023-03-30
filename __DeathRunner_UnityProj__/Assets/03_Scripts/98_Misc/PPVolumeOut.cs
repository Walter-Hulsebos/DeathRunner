using DG.Tweening;
using JetBrains.Annotations;
using Mono.CSharp;
using static Unity.Mathematics.math;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game
{
    public sealed class PPVolumeOut : MonoBehaviour
    {
        [SerializeField] private Volume volume;

        [SerializeField] private float duration = 0.5f;
        
        private bool isActivated = false;
        
        private float timeSpentTransitioning = 0;

        [UsedImplicitly]
        public void Out()
        {
            volume.weight = 1;

            timeSpentTransitioning = 0;
            
            isActivated = true;
        }
        
        public void Update()
        {
            if (!isActivated) return;
            
            timeSpentTransitioning += Time.unscaledDeltaTime;
            
            volume.weight = lerp(1, 0, timeSpentTransitioning / duration);

            if (volume.weight >= 1)
            {
                isActivated = false;
            }
        }
    }
}