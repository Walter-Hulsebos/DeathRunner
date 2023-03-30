using DG.Tweening;
using QFSW.QC;
using UnityEngine;

using F32  = System.Single;

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
        
        private const F32 SlowMoScale = 0.075f;
        private const F32 NormalScale = 1.0f;
        
        [Command]
        public static Bool IsSlowMotionEnabled
        {
            get => _isSlowMotionEnabled;
            set
            {
                Debug.Log(message: $"SlowMo: {value}");
                
                _isSlowMotionEnabled = value;
                Time.timeScale = _isSlowMotionEnabled ? SlowMoScale : NormalScale;
                Time.fixedDeltaTime = 0.0166667f * Time.timeScale;
                
                DOTween.timeScale = Time.timeScale;
                DOTween.useSmoothDeltaTime = false;
            }
        }
        
        public static F32 DeltaTime => IsSlowMotionEnabled ? Time.unscaledDeltaTime : Time.deltaTime;
    }
}