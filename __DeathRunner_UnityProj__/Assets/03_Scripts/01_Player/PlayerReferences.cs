using System;
using DeathRunner.Attributes;
using UnityEngine;

using JetBrains.Annotations;

using EasyCharacterMovement;

using DeathRunner.Inputs;

using Object = UnityEngine.Object;
using F32x3  = Unity.Mathematics.float3;
using Rotor  = Unity.Mathematics.quaternion;

namespace DeathRunner.Player
{
    [Serializable]
    public sealed class PlayerReferences
    {
        [field:SerializeField] public Camera           Camera       { get; [UsedImplicitly] private set; }
        [field:SerializeField] public CharacterMotor   Motor        { get; [UsedImplicitly] private set; }
        [field:SerializeField] public InputHandler     InputHandler { get; [UsedImplicitly] private set; }
        [field:SerializeField] public Transform        LookAt       { get; [UsedImplicitly] private set; }
        [field:SerializeField] public HealthComponent  Health       { get; [UsedImplicitly] private set; }
        //[field:SerializeField] public StaminaComponent Stamina      { get; [UsedImplicitly] private set; }

        public F32x3 WorldPos
        {
            get => (F32x3)Motor.GetPosition();
            set => Motor.SetPosition(newPosition: value);
        }
        // public F32x3 LocalPos
        // {
        //     get => transform.localPosition;
        //     set => transform.localPosition = value;
        // }
        
        public Rotor Rot
        {
            get => (Rotor)Motor.rotation;
            set => Motor.SetRotation(newRotation: value);
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
            
            FindCharacterMotor(gameObject: gameObject);
            
            FindInputHandler(gameObject: gameObject);
            
            FindHealthComponent(gameObject: gameObject);
            
            //FindStaminaComponent(gameObject: gameObject);
        }

        public void OnValidate(GameObject gameObject)
        {
            if (Camera == null)
            {
                FindPlayerCamera();
            }
            
            if (Motor == null)
            {
                FindCharacterMotor(gameObject: gameObject);
            }
            
            if (Motor != null)
            {
                // Enable default physic interactions
                Motor.enablePhysicsInteraction = true;    
            }
            
            if (InputHandler == null)
            {
                FindInputHandler(gameObject: gameObject);
            }
            
            if (Health == null)
            {
                FindHealthComponent(gameObject: gameObject);
            }
            
            // if (Stamina == null)
            // {
            //     FindStaminaComponent(gameObject: gameObject);
            // }
        }

        public void Init(GameObject gameObject)
        {
            Debug.Log(message: "<b><color=red>Before</color></b> Init Health, setting to <i>Max</i> \n" +
                               $"Health Max: {Health.health.Max.Value} \n" +
                               $"Health:     {Health.health.Value}", context: gameObject);
            Health.health.Init(owner: gameObject);
            Debug.Log(message: "<b><color=green>After</color></b> Init Health \n" +
                               $"Health:     {Health.health.Value}", context: gameObject);
            
            
            // Debug.Log(message: "<b><color=red>Before</color></b> Init Stamina, setting to <i>Max</i> \n" +
            //                    $"Stamina Max: {Stamina.stamina.Max.Value} \n" +
            //                    $"Stamina:     {Stamina.stamina.Value}", context: gameObject);
            // Stamina.stamina.Init(owner: gameObject);
            // Debug.Log(message: "<b><color=green>After</color></b> Init Stamina \n" +
            //                    $"Stamina:     {Stamina.stamina.Value}", context: gameObject);
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
            if (gameObject.TryGetComponent(component: out CharacterMotor __motor))
            {
                Motor = __motor;
            }
            else
            {
                Debug.LogError(message: $"No CharacterMotor found on {gameObject.name}", context: gameObject);
            }
        }
        
        private void FindInputHandler(GameObject gameObject)
        {
            //InputHandler = gameObject.GetComponent<InputHandler>();
            if (gameObject.TryGetComponent(component: out InputHandler __inputHandler))
            {
                InputHandler = __inputHandler;
            }
            else
            {
                Debug.LogError(message: $"No InputHandler found on {gameObject.name}", context: gameObject);
            }
        }

        private void FindHealthComponent(GameObject gameObject)
        {
            //Health = gameObject.GetComponent<HealthComponent>();
            if (gameObject.TryGetComponent(component: out HealthComponent __health))
            {
                Health = __health;
            }
            else
            {
                Debug.LogError(message: $"No HealthComponent found on {gameObject.name}", context: gameObject);
            }
        }
        
        // private void FindStaminaComponent(GameObject gameObject)
        // {
        //     //Stamina = gameObject.GetComponent<StaminaComponent>();
        //     if (gameObject.TryGetComponent(component: out StaminaComponent __stamina))
        //     {
        //         Stamina = __stamina;
        //     }
        //     else
        //     {
        //         Debug.LogError(message: $"No StaminaComponent found on {gameObject.name}", context: gameObject);
        //     }
        // }
    }
}
