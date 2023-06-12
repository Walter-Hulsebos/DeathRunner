using System;
using DG.Tweening;
using QFSW.QC;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static Unity.Mathematics.math;

using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;

using Bool  = System.Boolean;

namespace DeathRunner.Shared
{
    public static class Commands
    {
        static Commands()
        {
            //InputSystem.onBeforeUpdate += OnBeforeUpdate;
            InputSystem.onAfterUpdate  += CheckIfPlayerIsUsingAGamepad;
        }
        
        public static Bool PlayerIsUsingAGamepad { get; private set; }
        //public static Bool HasGamepadConnected => (Gamepad.all.Count > 0);

        private static void CheckIfPlayerIsUsingAGamepad()
        {
            //If there's no gamepad, set PlayerIsUsingAGamepad to false
            if(Gamepad.current == null)
            {
                PlayerIsUsingAGamepad = false;
                return;
            }
            
            //If there's any button input, set PlayerIsUsingAGamepad to true
            if 
            (
                (Gamepad.current.buttonEast.ReadValue()       > 0) ||
                (Gamepad.current.buttonWest.ReadValue()       > 0) ||
                (Gamepad.current.buttonNorth.ReadValue()      > 0) ||
                (Gamepad.current.buttonSouth.ReadValue()      > 0) ||
                (Gamepad.current.aButton.ReadValue()          > 0) ||
                (Gamepad.current.bButton.ReadValue()          > 0) ||
                (Gamepad.current.xButton.ReadValue()          > 0) ||
                (Gamepad.current.yButton.ReadValue()          > 0) ||
                (Gamepad.current.crossButton.ReadValue()      > 0) ||
                (Gamepad.current.circleButton.ReadValue()     > 0) ||
                (Gamepad.current.squareButton.ReadValue()     > 0) ||
                (Gamepad.current.triangleButton.ReadValue()   > 0) ||
                (Gamepad.current.leftShoulder.ReadValue()     > 0) ||
                (Gamepad.current.rightShoulder.ReadValue()    > 0) ||
                (Gamepad.current.leftTrigger.ReadValue()      > 0) ||
                (Gamepad.current.rightTrigger.ReadValue()     > 0) ||
                (Gamepad.current.leftStickButton.ReadValue()  > 0) ||
                (Gamepad.current.rightStickButton.ReadValue() > 0) ||
                (Gamepad.current.selectButton.ReadValue()     > 0) ||
                (Gamepad.current.startButton.ReadValue()      > 0) ||
                (Gamepad.current.dpad.left.ReadValue()        > 0) ||
                (Gamepad.current.dpad.right.ReadValue()       > 0) ||
                (Gamepad.current.dpad.down.ReadValue()        > 0) ||
                (Gamepad.current.dpad.up.ReadValue()          > 0)
            )
            {
                PlayerIsUsingAGamepad = true;
                return;
            }

            //If there's any left stick input, set PlayerIsUsingAGamepad to true
            if (Gamepad.current.leftStick.ReadValue() != Vector2.zero)
            {
                PlayerIsUsingAGamepad = true;
                return;
            }
            
            //If there's any right stick input, set PlayerIsUsingAGamepad to true
            if (Gamepad.current.rightStick.ReadValue() != Vector2.zero)
            {
                PlayerIsUsingAGamepad = true;
                return;
            }

            //If there's any mouse input, set PlayerIsUsingAGamepad to false, since the player is now using the mouse.
            if (Mouse.current == null) return;
            
            if (Mouse.current.delta.ReadValue() != Vector2.zero)
            {
                PlayerIsUsingAGamepad = false;
            }
        }

        // [Command]
        // public static void EnableSlowMotion()  => IsSlowMotionEnabled = true;
        // [Command]
        // public static void DisableSlowMotion() => IsSlowMotionEnabled = false;

        // private static Bool _isSlowMotionEnabled;
        //
        // [Command]
        // public static Bool IsSlowMotionEnabled
        // {
        //     get => _isSlowMotionEnabled;
        //     set
        //     {
        //         _isSlowMotionEnabled = value;
        //         
        //         Time.timeScale = _isSlowMotionEnabled ? 0.025f : 1.0f;
        //         Time.fixedDeltaTime = 0.0166667f * Time.timeScale;
        //         
        //         //DOTween.timeScale = Time.timeScale;
        //         //DOTween.useSmoothDeltaTime = false;
        //         
        //         Debug.Log(message: (_isSlowMotionEnabled ? "<b><color=green>SlowMo Enabled!</color></b>" : "<b><color=red>SnowMo Disabled!</color></b>") + $" TimeScale: {Time.timeScale}");
        //     }
        // }

        //public static F32 DeltaTime => IsSlowMotionEnabled ? Time.unscaledDeltaTime : Time.deltaTime;
    }
}