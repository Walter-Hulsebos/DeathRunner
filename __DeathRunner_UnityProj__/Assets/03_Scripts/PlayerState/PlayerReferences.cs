using System;
using DeathRunner.Inputs;
using EasyCharacterMovement;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

using F32x2 = Unity.Mathematics.float2;
using F32x3 = Unity.Mathematics.float3;
using Rotor = Unity.Mathematics.quaternion;

namespace DeathRunner.PlayerState
{
    [Serializable]
    public class PlayerReferences
    {
        [field:SerializeField] public Camera         Camera       { get; [UsedImplicitly] private set; }
        [field:SerializeField] public CharacterMotor Motor        { get; [UsedImplicitly] private set; }
        [field:SerializeField] public InputHandler   InputHandler { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Transform      LookAt       { get; [UsedImplicitly] private set; }

        public F32x3 WorldPos
        {
            get => (F32x3)Motor.GetPosition();
            set => Motor.SetPosition(value);
        }
        // public F32x3 LocalPos
        // {
        //     get => transform.localPosition;
        //     set => transform.localPosition = value;
        // }
        
        public Rotor Rot
        {
            get => (Rotor)Motor.rotation;
            set => Motor.SetRotation(value);
        }

        public F32x3 WorldScale
        {
            get => (F32x3)Motor.transform.lossyScale;
        }
        public F32x3 LocalScale
        {
            get => (F32x3)Motor.transform.localScale;
            set => Motor.transform.localScale = value;
        }

        public F32x3 Backward => -(F32x3)Motor.transform.forward;
        public F32x3 Forward  => +(F32x3)Motor.transform.forward;
        public F32x3 Left     => -(F32x3)Motor.transform.right;
        public F32x3 Right    => +(F32x3)Motor.transform.right;
        public F32x3 Down     => -(F32x3)Motor.transform.up;
        public F32x3 Up       => +(F32x3)Motor.transform.up;
        
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
