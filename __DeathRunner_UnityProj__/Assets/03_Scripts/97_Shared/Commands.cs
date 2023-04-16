using System;
using DG.Tweening;
using QFSW.QC;
using UnityEngine;
using UnityEngine.InputSystem;
using F32  = System.Single;

using Bool  = System.Boolean;

namespace DeathRunner.Shared
{
    public static class Commands
    {
        // static Commands()
        // {
        //     InputSystem.onDeviceChange += (device, change) =>
        //     {
        //         switch (change)
        //         {
        //             case InputDeviceChange.Added:
        //             case InputDeviceChange.Reconnected:
        //             case InputDeviceChange.Enabled:
        //             {
        //                 if (device is Gamepad)
        //                 {
        //                     Debug.Log("PlayerIsUsingAGamepad = TRUE");
        //                     PlayerIsUsingAGamepad = true;
        //                 }
        //
        //                 break;
        //             }
        //             case InputDeviceChange.Removed:
        //             case InputDeviceChange.Disconnected:
        //             case InputDeviceChange.Disabled:
        //             {
        //                 if(device is Gamepad)
        //                 {
        //                     Debug.Log("PlayerIsUsingAGamepad = FALSE");
        //                     PlayerIsUsingAGamepad = false;
        //                 }
        //                 
        //                 break;
        //             }
        //             case InputDeviceChange.UsageChanged:
        //             case InputDeviceChange.ConfigurationChanged:
        //             case InputDeviceChange.SoftReset:
        //             case InputDeviceChange.HardReset:
        //                 break;
        //             default:
        //                 throw new ArgumentOutOfRangeException(nameof(change), change, null);
        //         }
        //     };
        // }
        
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
        
        
        
        //public static Bool PlayerIsUsingAGamepad { get; private set; }
        
        public static Bool PlayerIsUsingAGamepad => (Gamepad.all.Count > 0);
        
        public static F32 DeltaTime => IsSlowMotionEnabled ? Time.unscaledDeltaTime : Time.deltaTime;
    }
}