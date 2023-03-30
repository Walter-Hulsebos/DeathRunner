using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game
{
    public class LerpVolumeWeight : MonoBehaviour
    {
        [SerializeField] private Volume volume;

        [SerializeField] private float duration = 0.5f;

        [UsedImplicitly]
        public void In()
        {
            volume.weight = 0;
            
            DOTween.To(getter: () => volume.weight, setter: f => volume.weight = f, endValue: 1, duration: duration);
        }
        
        [UsedImplicitly]
        public void Out()
        {
            volume.weight = 1;
            
            DOTween.To(getter: () => volume.weight, setter: f => volume.weight = f, endValue: 0, duration: duration);
        }
    }
}
