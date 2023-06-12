using Animancer;
using GenericScriptableArchitecture;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace DeathRunner
{
    public sealed class SlowMo : MonoBehaviour
    {
        [SerializeField] private ScriptableEvent enable;
        [SerializeField] private ScriptableEvent disable;
        
        [SerializeField] private MMF_Player playback;
        [SerializeField] private AnimancerComponent _animancerComponent;
        
        #if UNITY_EDITOR
        private void Reset()
        {
            playback = GetComponent<MMF_Player>();

            _animancerComponent = GetComponent<AnimancerComponent>();
        }
        
        private void OnValidate()
        {
            if (playback == null)
            {
                playback = GetComponent<MMF_Player>();
            }

            if (_animancerComponent == null)
            {
                _animancerComponent = GetComponent<AnimancerComponent>();
            }
        }
        #endif

        private void OnEnable()
        {
            if (playback != null)
            {
                enable  += playback.PlayFeedbacks;
                disable += playback.StopFeedbacks;
            }
            
            if (_animancerComponent != null)
            {
                enable  += AnimancerSlowMoStart;
                disable += AnimancerSlowMoStop;
            }
        }
        
        private void OnDisable()
        {
            if (playback != null)
            {
                enable  -= playback.PlayFeedbacks;
                disable -= playback.StopFeedbacks;
            }
            
            if (_animancerComponent != null)
            {
                enable  -= AnimancerSlowMoStart;
                disable -= AnimancerSlowMoStop;
            }
        }

        private void AnimancerSlowMoStart()
        {
            _animancerComponent.UpdateMode = AnimatorUpdateMode.UnscaledTime;
        }
        
        private void AnimancerSlowMoStop()
        {
            _animancerComponent.UpdateMode = AnimatorUpdateMode.Normal;
        }
    }
}
