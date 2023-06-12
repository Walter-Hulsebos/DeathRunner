using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeathRunner
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;
        
        public Vector2 NavigationInput { get; set; }

        private InputAction _navigationAction;
        
        public static PlayerInput PlayerInput { get; set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            PlayerInput = GetComponent<PlayerInput>();
            _navigationAction = PlayerInput.actions["Navigate"];
        }

        private void Update()
        {
            NavigationInput = _navigationAction.ReadValue<Vector2>();
        }
    }
}
