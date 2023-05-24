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
        [field:SerializeField] public Constant<U16> Max         { get; [UsedImplicitly] private set; }
        
        #if ODIN_INSPECTOR
        [field:BoxGroup("Current", showLabel: false)]
        #endif  
        [field:SerializeField] public Bool          UseInfinity { get; [UsedImplicitly] private set; }
        
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
                
                if (OnChanged != null)
                {
                    OnChanged.Invoke(__previous, currentStaminaBackingField.Value);   
                }

                if (currentStaminaBackingField.Value > __previous)
                {
                    if (OnIncreased != null)
                    {
                        OnIncreased.Invoke(__previous, currentStaminaBackingField.Value);
                    }
                }
                else if (currentStaminaBackingField.Value < __previous)
                {
                    if (OnDecreased != null)
                    {
                        OnDecreased.Invoke(__previous, currentStaminaBackingField.Value);
                    }
                }
                
                if (currentStaminaBackingField.Value == 0)
                {
                    if (OnDepleted != null)
                    {
                        OnDepleted.Invoke();   
                    }
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
    }
}
