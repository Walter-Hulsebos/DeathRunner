using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EasyCharacterMovement;
using Game.Inputs;
using Sirenix.OdinInspector;
using UnityEngine;

using F32   = System.Single;
using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;

using static ProjectDawn.Mathematics.math2;
using static Unity.Mathematics.math;

namespace Game.Movement
{
    [RequireComponent(typeof(InputHandler), typeof(CharacterMotor))]
    
    public class Dash : MonoBehaviour
    {
        // References to other components
        private InputHandler inputHandler;
        private CharacterMotor motor;
        private Locomotion _locomotion;
        private Orientation _orientation;
        [SerializeField] private Camera playerCamera;
        
        // Dash input and direction
        private bool dashInput;
        private F32x3 dashDir;

        // Dash cooldown and status
        private bool canDash = true;
        private bool isDashing = false;

        private F32x3 relativeDashDir;
        
        [SerializeField] float dashTime = 1; // Length of the dash in seconds
        [SerializeField] float dashSpeed = 8; //Speed of dash
        
        [SerializeField, HideInInspector] private WaitForSeconds _dashWait;
        // Initialization
        void Start()
        {
       
        }

        // Update is called once per frame
        private async UniTask Update()
        {
            // Check if dash input is given and able to dash
            if (inputHandler.DashInput && canDash)
            {
                Debug.Log("dashing");
                canDash = false;
                isDashing = true;

                // Get dash direction from input
                dashDir = new Vector3(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y);

                // If no input is given, dash in the direction the player is facing
                if (all(dashDir == F32x3.zero))
                {
                    dashDir = new F32x3(x: 0, y: 0, z: -1);
                }

                // Convert dash direction to be relative to the player camera
                relativeDashDir = dashDir.RelativeTo(playerCamera.transform);

                // Disable locomotion and orientation while dashing
                _locomotion.enabled = false;
                _orientation.enabled = false;
                
                await UniTask.Delay(TimeSpan.FromSeconds(dashTime), ignoreTimeScale: false);

                // Reset dash direction and status, and enable locomotion and orientation
                dashDir = Vector3.zero;
                isDashing = false;
                _locomotion.enabled = true;
                _orientation.enabled = true;
                canDash = true;
                
                // Start the coroutine to end the dash

            }

            if (isDashing)
            {
                // Move the player in the dash direction
                motor.Move(relativeDashDir * dashSpeed);
            }
        }

    #if UNITY_EDITOR
        private void Reset()
        {
            FindInputHandler();
        }

        private void OnValidate()
        {

            if(inputHandler == null)
            {
                FindInputHandler();
            }
            //do this for every single one of those.
        }

        private void FindInputHandler()
        {
            inputHandler = GetComponent<InputHandler>(); 
            motor = GetComponent<CharacterMotor>();
            _locomotion = GetComponent<Locomotion>();
            _orientation = GetComponent<Orientation>();
        }
    #endif
        
    }
}
