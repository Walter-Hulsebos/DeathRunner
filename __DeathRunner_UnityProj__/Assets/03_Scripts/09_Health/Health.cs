using System;
using Cysharp.Threading.Tasks;
using GenericScriptableArchitecture;
using JetBrains.Annotations;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using UnityEngine;
using static ProjectDawn.Mathematics.math2;

using F32  = System.Single; //max 3.402823E+38
using U16  = System.UInt16; //max 65,535
using Bool = System.Boolean;

namespace DeathRunner.Attributes
{
    [Serializable]
    public struct Health : IChangeable<U16>, IDamageable
    {
        [field:SerializeField] public Constant<U16> Max { get; [UsedImplicitly] private set; }
        
        #if ODIN_INSPECTOR
        [field:BoxGroup("IFrames", showLabel: false)]
        #endif        
        [field:SerializeField] public Reference<F32> InvincibilityFrameDuration { get; [UsedImplicitly] private set; }

        #if ODIN_INSPECTOR
        [BoxGroup("Current", showLabel: false)]
        [LabelText("Is Invincible")]
        #endif
        [SerializeField] private Reference<Bool> useInfinityBackingField;
        
        public Bool UseInfinity
        {
            get => useInfinityBackingField.Value;
            set
            {
                Bool __valueHasChanged = (useInfinityBackingField.Value != value);
                
                if (!__valueHasChanged) return;
                
                useInfinityBackingField.Value = value;

                if (value)
                {
                    OnInvincibilityEnabled?.Invoke();
                }
                else
                {
                    OnInvincibilityDisabled?.Invoke();
                }
            }
        }

        #if ODIN_INSPECTOR
        [BoxGroup("Current", showLabel: false)]
        [LabelText("Current Health")]
        #endif
        [SerializeField] private Reference<U16> currentHealthBackingField;
        
        public U16 Value 
        {
            get => UseInfinity ? Max.Value : currentHealthBackingField.Value;
            set
            {
                if (UseInfinity) 
                {
                    if (value < Max.Value)
                    {
                        OnDecreasedAttempt?.Invoke();
                    }
                    else
                    {
                        OnIncreasedAttempt?.Invoke();
                    }

                    return; // If UseInfinity is true, exit. We don't want to change the value.
                }  
                
                #if UNITY_EDITOR
                Debug.Log($"Attempting Health Change [{currentHealthBackingField.Value}] → [{value}]");
                #endif
                
                value = clamp(target: value, a: 0, b: Max.Value); //Make sure we don't go over the max 
                
                // Exit if the value hasn't changed.
                if (value == currentHealthBackingField.Value)
                {
                    #if UNITY_EDITOR
                    Debug.Log($"Health Change Aborted, is already [{value}]");
                    #endif
                    return;
                }

                U16 __previous = currentHealthBackingField.Value;
                // Set the new value.
                currentHealthBackingField.Value = value;

                #if UNITY_EDITOR
                Debug.Log($"Health Changed [{__previous}] → [{currentHealthBackingField.Value}]");
                #endif

                OnChanged?.Invoke(__previous, currentHealthBackingField.Value);

                if (currentHealthBackingField.Value > __previous)
                {
                    OnIncreased?.Invoke(__previous, currentHealthBackingField.Value);
                }
                else if (currentHealthBackingField.Value < __previous)
                {
                    // IFrames
                    InvincibilityFrames().Forget();

                    OnDecreased?.Invoke(__previous, currentHealthBackingField.Value);
                }
                
                if (currentHealthBackingField.Value == 0)
                {
                    OnDepleted?.Invoke();
                }
            }
        }
        
        public async UniTask InvincibilityFrames()
        {
            if (InvincibilityFrameDuration.Value == 0f) return;
            
            Debug.Log($"InvincibilityFrames for [{InvincibilityFrameDuration.Value}] seconds");
            UseInfinity = true;
            await UniTask.Delay(TimeSpan.FromSeconds(InvincibilityFrameDuration.Value));
            UseInfinity = false;
            Debug.Log($"InvincibilityFrames ended");
        }

        public void Init()
        {
            currentHealthBackingField.Value = Max.Value;
            
            switch (currentHealthBackingField.Type)
            {
                case BaseReference.ValueType.Variable:
                    Debug.Log("Health: Init() - Variable");
                    currentHealthBackingField.VariableValue.Value = Max.Value;
                    break;
                case BaseReference.ValueType.VariableInstancer:
                    Debug.Log("Health: Init() - VariableInstancer");
                    currentHealthBackingField.InstancerValue.Value = Max.Value;
                    break;
                case BaseReference.ValueType.Value:
                    Debug.Log("Health: Init() - Value");
                    currentHealthBackingField.Value = Max.Value;
                    break;
                case BaseReference.ValueType.Constant:
                    Debug.Log("Health: Init() - Constant");
                    Debug.LogWarning("Cannot set a constant value");
                    break;
            }
        }
        
        //[OdinSerialize]
        //public IMod<U16>[] Modifiers { get; [UsedImplicitly] private set; }

        [field:SerializeField] public EventReference<UInt16, UInt16> OnChanged               { get; [UsedImplicitly] private set; }
        [field:SerializeField] public EventReference<UInt16, UInt16> OnDecreased             { get; [UsedImplicitly] private set; }
        [field:SerializeField] public EventReference                 OnDepleted              { get; [UsedImplicitly] private set; }
        [field:SerializeField] public EventReference<UInt16, UInt16> OnIncreased             { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public EventReference                 OnInvincibilityEnabled  { get; [UsedImplicitly] private set; }
        [field:SerializeField] public EventReference                 OnInvincibilityDisabled { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public EventReference                 OnDecreasedAttempt      { get; [UsedImplicitly] private set; }
        [field:SerializeField] public EventReference                 OnIncreasedAttempt      { get; [UsedImplicitly] private set; }

        public Bool IsZero => Value == 0;
    }
}
