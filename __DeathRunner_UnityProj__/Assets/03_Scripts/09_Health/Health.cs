using System;
using DeathRunner.Attributes.Modifiers;
using GenericScriptableArchitecture;
using JetBrains.Annotations;
using Sirenix.Serialization;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using static ProjectDawn.Mathematics.math2;

using U16  = System.UInt16; //max 65,535
using Bool = System.Boolean;

namespace DeathRunner.Attributes
{
    public struct Health : IChangeable<U16>, IDamageable
    {
        [field:SerializeField]
        public Constant<U16> Max { get; [UsedImplicitly] private set; }
        
        #if ODIN_INSPECTOR
        [LabelText("Current Health")]
        #endif
        [field:SerializeField] private Variable<U16> currentHealthBackingField;
        public U16 Value 
        {
            get => currentHealthBackingField.Value;
            set
            {
                if(value == currentHealthBackingField.Value) return;
                
                value = min(value, Max.Value); //Make sure we don't go over the max
                
                if (value == currentHealthBackingField.Value) return;
                
                U16 __previous = currentHealthBackingField.Value;
                currentHealthBackingField.Value = value;
                
                if (OnChanged != null)
                {
                    OnChanged.Invoke(__previous, currentHealthBackingField.Value);   
                }

                if (currentHealthBackingField.Value > __previous)
                {
                    if (OnIncreased != null)
                    {
                        OnIncreased.Invoke();
                    }
                }
                else if (currentHealthBackingField.Value < __previous)
                {
                    if (OnDecreased != null)
                    {
                        OnDecreased.Invoke();
                    }
                }
                
                if (currentHealthBackingField == 0)
                {
                    if (OnDepleted != null)
                    {
                        OnDepleted.Invoke();   
                    }
                }
            }
        }
        
        [OdinSerialize]
        public IMod<U16>[] Modifiers { get; [UsedImplicitly] private set; }

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
