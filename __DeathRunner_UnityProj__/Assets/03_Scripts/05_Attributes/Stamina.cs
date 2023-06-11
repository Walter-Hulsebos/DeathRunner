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
        [field:SerializeField] public Constant<U16> Max { get; [UsedImplicitly] private set; }
        
        #if ODIN_INSPECTOR
        [BoxGroup("Current", showLabel: false)]
        [LabelText(text: "Use Infinite Stamina")]
        [SerializeField] 
        #endif
        public Reference<Bool> useInfinityBackingField;
        
        public Bool UseInfinity { get => useInfinityBackingField.Value; private set => useInfinityBackingField.Value = value; }
        
        #if ODIN_INSPECTOR
        [field:BoxGroup("Current", showLabel: false)]
        [field:LabelText(text: "Current Stamina")]
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
        
        #if UNITY_EDITOR
        private Bool _hasOwnerObject;
        #endif
        private UnityEngine.Object _ownerObject;


        public void Init()
        {
            currentStaminaBackingField.Value = Max.Value;
            
            switch (currentStaminaBackingField.Type)
            {
                case BaseReference.ValueType.Variable:
                    StaminaVariable();
                    break;
                case BaseReference.ValueType.VariableInstancer:
                    StaminaVariableInstancer();
                    break;
                case BaseReference.ValueType.Value:
                    StaminaValue();
                    break;
                case BaseReference.ValueType.Constant:
                    StaminaConstant();
                    break;
            }
        }

        public void Init(UnityEngine.Object owner)
        {
            if (owner != null)
            {
                #if UNITY_EDITOR
                _hasOwnerObject = true;
                #endif
                _ownerObject = owner;
                
                #if UNITY_EDITOR
                Debug.Log(message: $"{nameof(Stamina)}: Init <b>with</b> owner object! ({_ownerObject.name})", context: _ownerObject);
                #endif
            }
            else
            {
                #if UNITY_EDITOR
                _hasOwnerObject = false;
                #endif
                
                #if UNITY_EDITOR
                Debug.LogWarning(message: $"{nameof(Stamina)}: Init with owner object, but owner object is null!");
                #endif
            }
            
            Init();
        }

        private void StaminaConstant()
        {
            #if UNITY_EDITOR
            if (_hasOwnerObject)
            {
                Debug.Log(message: $"{nameof(Stamina)}: (<i>{currentStaminaBackingField.ConstantValue.name}</i>): Init() - Constant", context: _ownerObject);
                Debug.LogWarning(message: "Cannot set a constant value!", context: _ownerObject);
            }
            else
            {
                Debug.Log(message: $"{nameof(Stamina)}: (<i>{currentStaminaBackingField.ConstantValue.name}</i>): Init() - Constant");
                Debug.LogWarning(message: "Cannot set a constant value!", context: _ownerObject);
            }
            #endif
        }

        private void StaminaValue()
        {
            #if UNITY_EDITOR
            if (_hasOwnerObject)
            {
                Debug.Log(message: nameof(Stamina) + ": Init() - Value", context: _ownerObject);
            }
            else
            {
                Debug.Log(message: nameof(Stamina) + ": Init() - Value");
            }
            #endif
            currentStaminaBackingField.Value = Max.Value;
        }

        private void StaminaVariableInstancer()
        {
            #if UNITY_EDITOR
            if (_hasOwnerObject)
            {
                Debug.Log(message: $"{nameof(Stamina)} (<i>{currentStaminaBackingField.InstancerValue.name}</i>): Init() - VariableInstancer", context: _ownerObject);
            }
            else
            {
                Debug.Log(message: $"{nameof(Stamina)} (<i>{currentStaminaBackingField.InstancerValue.name}</i>): Init() - VariableInstancer");
            }
            #endif
            currentStaminaBackingField.InstancerValue.Value = Max.Value;
        }

        private void StaminaVariable()
        {
            #if UNITY_EDITOR
            if (_hasOwnerObject)
            {
                Debug.Log(message: $"{nameof(Stamina)} (<i>{currentStaminaBackingField.VariableValue.name}</i>): Init() - Variable", context: _ownerObject);
            }
            else
            {
                Debug.Log(message: $"{nameof(Stamina)} (<i>{currentStaminaBackingField.VariableValue.name}</i>): Init() - Variable");
            }
            #endif
            currentStaminaBackingField.VariableValue.Value = Max.Value;
        }
        
    }
}
