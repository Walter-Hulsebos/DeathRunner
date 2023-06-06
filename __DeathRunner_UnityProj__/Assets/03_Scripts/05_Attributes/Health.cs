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
        [field:BoxGroup(@group: "IFrames", showLabel: false)]
        #endif        
        [field:SerializeField] public Reference<F32> InvincibilityFrameDuration { get; [UsedImplicitly] private set; }

        #if ODIN_INSPECTOR
        [BoxGroup(@group: "Current", showLabel: false)]
        [LabelText(text: "Is Invincible")]
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
        [BoxGroup(@group: "Current", showLabel: false)]
        [LabelText(text: "Current Health")]
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
                if (_hasOwnerObject)
                {
                    Debug.Log(message: $"Attempting Health Change [{currentHealthBackingField.Value}] → [{value}]", context: _ownerObject);    
                }
                else
                {
                    Debug.Log(message: $"Attempting Health Change [{currentHealthBackingField.Value}] → [{value}]");
                }
                #endif
                
                value = clamp(target: value, a: 0, b: Max.Value); //Make sure we don't go over the max 
                
                // Exit if the value hasn't changed.
                if (value == currentHealthBackingField.Value)
                {
                    #if UNITY_EDITOR
                    if (_hasOwnerObject)
                    {
                        Debug.Log(message: $"Health Change Aborted, is already [{value}]", context: _ownerObject);
                    }
                    else
                    {
                        Debug.Log(message: $"Health Change Aborted, is already [{value}]");
                    }
                    #endif
                    
                    return;
                }

                U16 __previous = currentHealthBackingField.Value;
                // Set the new value.
                currentHealthBackingField.Value = value;

                #if UNITY_EDITOR
                if (_hasOwnerObject)
                {
                    Debug.Log(message: $"Health Changed [{__previous}] → [{currentHealthBackingField.Value}]", context: _ownerObject);   
                }
                else
                {
                    Debug.Log(message: $"Health Changed [{__previous}] → [{currentHealthBackingField.Value}]");
                }
                #endif

                OnChanged?.Invoke(arg0: __previous, arg1: currentHealthBackingField.Value);

                if (currentHealthBackingField.Value > __previous)
                {
                    OnIncreased?.Invoke(arg0: __previous, arg1: currentHealthBackingField.Value);
                }
                else if (currentHealthBackingField.Value < __previous)
                {
                    // IFrames
                    InvincibilityFrames().Forget();

                    OnDecreased?.Invoke(arg0: __previous, arg1: currentHealthBackingField.Value);
                }
                
                if (currentHealthBackingField.Value == 0)
                {
                    OnDepleted?.Invoke();
                }
            }
        }
        
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

        private Bool _hasOwnerObject;
        private UnityEngine.Object _ownerObject;
        
        public async UniTask InvincibilityFrames()
        {
            if (InvincibilityFrameDuration.Value == 0f) return;

            #if UNITY_EDITOR
            if (_hasOwnerObject)
            {
                Debug.Log(message: $"InvincibilityFrames for [{InvincibilityFrameDuration.Value}] seconds", context: _ownerObject);
            }
            else
            {
                Debug.Log(message: $"InvincibilityFrames for [{InvincibilityFrameDuration.Value}] seconds");
            }
            #endif
            
            UseInfinity = true;
            await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(value: InvincibilityFrameDuration.Value));
            UseInfinity = false;
            
            #if UNITY_EDITOR
            if (_hasOwnerObject)
            {
                Debug.Log(message: $"InvincibilityFrames ended", context: _ownerObject);
            }
            else
            {
                Debug.Log(message: $"InvincibilityFrames ended");
            }
            #endif
        }

        public void Init()
        {
            currentHealthBackingField.Value = Max.Value;
            
            switch (currentHealthBackingField.Type)
            {
                case BaseReference.ValueType.Variable:
                    HealthVariable();
                    break;
                case BaseReference.ValueType.VariableInstancer:
                    HealthVariableInstancer();
                    break;
                case BaseReference.ValueType.Value:
                    HealthValue();
                    break;
                case BaseReference.ValueType.Constant:
                    HealthConstant();
                    break;
            }
        }
        
        public void Init(UnityEngine.Object owner)
        {
            if (owner != null)
            {
                _hasOwnerObject = true;
                _ownerObject = owner;
                
                #if UNITY_EDITOR
                Debug.Log(message: $"{nameof(Health)}: Init <b>with</b> owner object! ({_ownerObject.name})", context: _ownerObject);
                #endif
            }
            else
            {
                _hasOwnerObject = false;
                
                #if UNITY_EDITOR
                Debug.LogWarning(message: $"{nameof(Health)}: Init with owner object, but owner object is null!");
                #endif
            }
            
            Init();
        }

        private void HealthConstant()
        {
            #if UNITY_EDITOR
            if (_hasOwnerObject)
            {
                Debug.Log(message: $"{nameof(Health)}: (<i>{currentHealthBackingField.ConstantValue.name}</i>): Init() - Constant", context: _ownerObject);
                Debug.LogWarning(message: "Cannot set a constant value!", context: _ownerObject);
            }
            else
            {
                Debug.Log(message: $"{nameof(Health)}: (<i>{currentHealthBackingField.ConstantValue.name}</i>): Init() - Constant");
                Debug.LogWarning(message: "Cannot set a constant value!", context: _ownerObject);
            }
            #endif
        }

        private void HealthValue()
        {
            #if UNITY_EDITOR
            if (_hasOwnerObject)
            {
                Debug.Log(message: nameof(Health) + ": Init() - Value", context: _ownerObject);
            }
            else
            {
                Debug.Log(message: nameof(Health) + ": Init() - Value");
            }
            #endif
            currentHealthBackingField.Value = Max.Value;
        }

        private void HealthVariableInstancer()
        {
            #if UNITY_EDITOR
            if (_hasOwnerObject)
            {
                Debug.Log(message: $"{nameof(Health)} (<i>{currentHealthBackingField.InstancerValue.name}</i>): Init() - VariableInstancer", context: _ownerObject);
            }
            else
            {
                Debug.Log(message: $"{nameof(Health)} (<i>{currentHealthBackingField.InstancerValue.name}</i>): Init() - VariableInstancer");
            }
            #endif
            currentHealthBackingField.InstancerValue.Value = Max.Value;
        }

        private void HealthVariable()
        {
            #if UNITY_EDITOR
            if (_hasOwnerObject)
            {
                Debug.Log(message: $"{nameof(Health)} (<i>{currentHealthBackingField.VariableValue.name}</i>): Init() - Variable", context: _ownerObject);
            }
            else
            {
                Debug.Log(message: $"{nameof(Health)} (<i>{currentHealthBackingField.VariableValue.name}</i>): Init() - Variable");
            }
            #endif
            currentHealthBackingField.VariableValue.Value = Max.Value;
        }
    }
}
