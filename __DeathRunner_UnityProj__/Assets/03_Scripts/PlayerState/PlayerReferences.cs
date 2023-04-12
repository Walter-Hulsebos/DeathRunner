using System;
using DeathRunner.Inputs;
using EasyCharacterMovement;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DeathRunner.PlayerState
{
    [Serializable]
    public class PlayerReferences
    {
        [field:SerializeField] public Camera         Camera       { get; [UsedImplicitly] private set; }
        [field:SerializeField] public CharacterMotor Motor        { get; [UsedImplicitly] private set; }
        [field:SerializeField] public InputHandler   InputHandler { get; [UsedImplicitly] private set; }
        
        public void Reset(GameObject gameObject)
        {
            FindPlayerCamera();
            
            FindCharacterMotor(gameObject);
            
            FindInputHandler(gameObject);
        }

        public void OnValidate(GameObject gameObject)
        {
            if (Camera == null)
            {
                FindPlayerCamera();
            }
            
            if (Motor == null)
            {
                FindCharacterMotor(gameObject);
            }
            
            if (Motor != null)
            {
                // Enable default physic interactions
                Motor.enablePhysicsInteraction = true;    
            }
            
            if (InputHandler == null)
            {
                FindInputHandler(gameObject);
            }
        }

        private void FindPlayerCamera()
        {
            Camera = Camera.main;
            
            if (Camera == null)
            {
                Camera = Object.FindObjectOfType<Camera>();
            }
        }

        private void FindCharacterMotor(GameObject gameObject)
        {
            Motor = gameObject.GetComponent<CharacterMotor>();
        }
        
        private void FindInputHandler(GameObject gameObject)
        {
            InputHandler = gameObject.GetComponent<InputHandler>();
        }
    }
}
