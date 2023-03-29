using DG.Tweening;
using QFSW.QC;
using UnityEngine;

using Bool  = System.Boolean;

namespace DeathRunner.Shared
{
    public static class Commands
    {
        [Command]
        public static void EnableSlowMotion() => IsSlowMotionEnabled = true;

        [Command]
        public static void DisableSlowMotion() => IsSlowMotionEnabled = false;

        private static Bool _isSlowMotionEnabled;
        
        [Command]
        public static Bool IsSlowMotionEnabled
        {
            get => _isSlowMotionEnabled;
            set
            {
                Debug.Log(message: $"SlowMo: {value}");
                
                _isSlowMotionEnabled = value;
                Time.timeScale = _isSlowMotionEnabled ? 0.025f : 1.0f;
                Time.fixedDeltaTime = 0.0166667f * Time.timeScale;
                
                DOTween.timeScale = Time.timeScale;
                DOTween.useSmoothDeltaTime = false;
            }
        }
    }
}