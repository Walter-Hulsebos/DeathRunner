using DG.Tweening;
using JetBrains.Annotations;
using Mono.CSharp;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game
{
    public sealed class PPVolumeIn : MonoBehaviour
    {
        [SerializeField] private Volume volume;

        [SerializeField] private float duration = 0.5f;
        
        private bool isActivated = false;
        
        private float timeSpentTransitioning = 0;

        [UsedImplicitly]
        public void In()
        {
            volume.weight = 0;

            timeSpentTransitioning = 0;
            
            isActivated = true;
        }
        
        public void Update()
        {
            if (!isActivated) return;
            
            timeSpentTransitioning += Time.unscaledDeltaTime;
            
            volume.weight = Mathf.Lerp(0, 1, timeSpentTransitioning / duration);

            if (volume.weight >= 1)
            {
                isActivated = false;
            }
        }
    }
}