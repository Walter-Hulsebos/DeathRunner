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
        
        #if UNITY_EDITOR
        private void Reset()
        {
            playback = FindObjectOfType<MMF_Player>();
        }
        
        private void OnValidate()
        {
            if (playback == null)
            {
                playback = FindObjectOfType<MMF_Player>();
            }
        }
        #endif

        private void OnEnable()
        {
            enable  += playback.PlayFeedbacks;
            disable += playback.StopFeedbacks;
        }
        
        private void OnDisable()
        {
            enable  -= playback.PlayFeedbacks;
            disable -= playback.StopFeedbacks;
        }
    }
}
