using System;
using UnityEngine;
using JetBrains.Annotations;
using GenericScriptableArchitecture;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using static ProjectDawn.Mathematics.math2;

using U16  = System.UInt16; //max 65,535
using Bool = System.Boolean;

namespace DeathRunner.Health
{
    [Serializable]
    public struct Health : IChangeable<U16>, IDamageable
    {
        [field:SerializeField]
        public Constant<U16> Max { get; [UsedImplicitly] private set; }
        
        #if ODIN_INSPECTOR
        [LabelText("Current Health")]
        #endif
        [field:SerializeField] private Variable<U16> _currentHealthBackingField;
        public U16 Current 
        {
            get => _currentHealthBackingField.Value;
            private set
            {
                if(value == _currentHealthBackingField.Value) return;
                
                value = min(value, Max.Value); //Make sure we don't go over the max
                
                if (value == _currentHealthBackingField.Value) return;
                
                U16 __previous = _currentHealthBackingField.Value;
                _currentHealthBackingField.Value = value;
                
                if (OnChanged != null)
                {
                    OnChanged.Invoke(__previous, _currentHealthBackingField.Value);   
                }

                if (_currentHealthBackingField.Value > __previous)
                {
                    if (OnIncreased != null)
                    {
                        OnIncreased.Invoke();
                    }
                }
                else if (_currentHealthBackingField.Value < __previous)
                {
                    if (OnDecreased != null)
                    {
                        OnDecreased.Invoke();
                    }
                }
                
                if (_currentHealthBackingField == 0)
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

        public Bool IsZero => Current == 0;
    }
}
