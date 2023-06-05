using System;
using DeathRunner.Attributes.Modifiers;
using GenericScriptableArchitecture;
using JetBrains.Annotations;
using Sirenix.Serialization;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using UnityEngine;

using static ProjectDawn.Mathematics.math2;

using U16  = System.UInt16; //max 65,535
using Bool = System.Boolean;

namespace DeathRunner.Attributes
{
    [Serializable]
    public struct Stamina : IChangeable<U16>
    {
        [field:SerializeField] public Constant<U16>   Max         { get; [UsedImplicitly] private set; }
        
        #if ODIN_INSPECTOR
        [BoxGroup("Current", showLabel: false)]
        [SerializeField] 
        #endif
        public Reference<Bool> useInfinityBackingField;
        
        public Bool UseInfinity { get => useInfinityBackingField.Value; private set => useInfinityBackingField.Value = value; }
        
        #if ODIN_INSPECTOR
        [LabelText("Current Health")]
        [BoxGroup("Current", showLabel: false)]
        #endif
        [field:SerializeField] private Reference<U16> currentStaminaBackingField;
        public U16 Value 
        {
            get => currentStaminaBackingField.Value;
            set
            {
                if(value == currentStaminaBackingField.Value) return;
                
                value = min(value, Max.Value); //Make sure we don't go over the max
                
                if (value == currentStaminaBackingField.Value) return;
                
                U16 __previous = currentStaminaBackingField.Value;
                currentStaminaBackingField.Value = value;

                OnChanged?.Invoke(__previous, currentStaminaBackingField.Value);

                if (currentStaminaBackingField.Value > __previous)
                {
                    OnIncreased?.Invoke(__previous, currentStaminaBackingField.Value);
                }
                else if (currentStaminaBackingField.Value < __previous)
                {
                    OnDecreased?.Invoke(__previous, currentStaminaBackingField.Value);
                }
                
                if (currentStaminaBackingField.Value == 0)
                {
                    OnDepleted?.Invoke();
                }
            }
        }
        
        //[OdinSerialize]
        //public IMod<U16>[] Modifiers { get; [UsedImplicitly] private set; }

        [field:SerializeField] public EventReference<UInt16, UInt16> OnChanged   { get; [UsedImplicitly] private set; }
        [field:SerializeField] public EventReference<UInt16, UInt16> OnDecreased { get; [UsedImplicitly] private set; }
        [field:SerializeField] public EventReference                 OnDepleted  { get; [UsedImplicitly] private set; }
        [field:SerializeField] public EventReference<UInt16, UInt16> OnIncreased { get; [UsedImplicitly] private set; }

        public Bool IsZero => Value == 0;
        
        public void Init()
        {
            currentStaminaBackingField.Value = Max.Value;
            
            switch (currentStaminaBackingField.Type)
            {
                case BaseReference.ValueType.Variable:
                    Debug.Log("Stamina: Init() - Variable");
                    currentStaminaBackingField.VariableValue.Value = Max.Value;
                    break;
                case BaseReference.ValueType.VariableInstancer:
                    Debug.Log("Stamina: Init() - VariableInstancer");
                    currentStaminaBackingField.InstancerValue.Value = Max.Value;
                    break;
                case BaseReference.ValueType.Value:
                    Debug.Log("Stamina: Init() - Value");
                    currentStaminaBackingField.Value = Max.Value;
                    break;
                case BaseReference.ValueType.Constant:
                    Debug.Log("Stamina: Init() - Constant");
                    Debug.LogWarning("Cannot set a constant value");
                    break;
            }
        }
    }
}
