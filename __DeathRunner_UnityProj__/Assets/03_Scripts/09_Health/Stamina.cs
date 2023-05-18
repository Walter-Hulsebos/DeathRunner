using System;
using GenericScriptableArchitecture;
using JetBrains.Annotations;

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
        [field:SerializeField]
        public Constant<U16> Max { get; [UsedImplicitly] private set; }
        
        #if ODIN_INSPECTOR
        [LabelText("Current Health")]
        #endif
        [field:SerializeField] private Variable<U16> currentStaminaBackingField;
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
                        OnIncreased.Invoke();
                    }
                }
                else if (currentStaminaBackingField.Value < __previous)
                {
                    if (OnDecreased != null)
                    {
                        OnDecreased.Invoke();
                    }
                }
                
                if (currentStaminaBackingField == 0)
                {
                    if (OnDepleted != null)
                    {
                        OnDepleted.Invoke();   
                    }
                }
            }
        }

        [field:SerializeField]
        public ScriptableEvent<UInt16, UInt16> OnChanged   { get; [UsedImplicitly] private set; }
        [field:SerializeField]
        public ScriptableEvent                 OnDecreased { get; [UsedImplicitly] private set; }
        [field:SerializeField]
        public ScriptableEvent                 OnIncreased { get; [UsedImplicitly] private set; }
        [field:SerializeField]
        public ScriptableEvent                 OnDepleted  { get; [UsedImplicitly] private set; }

        public Bool IsZero => Value == 0;
    }
}
