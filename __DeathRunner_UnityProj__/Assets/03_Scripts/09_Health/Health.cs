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
                    if (OnInvincibilityEnabled != null)
                    {
                        OnInvincibilityEnabled.Invoke();
                    }
                }
                else
                {
                    if (OnInvincibilityDisabled != null)
                    {
                        OnInvincibilityDisabled.Invoke();
                    }
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
                if (UseInfinity) return; // If UseInfinity is true, exit. We don't want to change the value.
                
                value = min(value, Max.Value); //Make sure we don't go over the max 
                
                // Exit if the value hasn't changed.
                if (value == currentHealthBackingField.Value) return;

                U16 __previous = currentHealthBackingField.Value;
                // Set the new value.
                currentHealthBackingField.Value = value;
                
                // IFrames
                InvincibilityFrames().Forget();
                
                Debug.Log($"Health Changed [{__previous}] → [{currentHealthBackingField.Value}]");
                if (OnChanged != null)
                {
                    OnChanged.Invoke(__previous, currentHealthBackingField.Value);   
                }

                if (currentHealthBackingField.Value > __previous)
                {
                    if (OnIncreased != null)
                    {
                        OnIncreased.Invoke(__previous, currentHealthBackingField.Value);
                    }
                }
                else if (currentHealthBackingField.Value < __previous)
                {
                    if (OnDecreased != null)
                    {
                        OnDecreased.Invoke(__previous, currentHealthBackingField.Value);
                    }
                }
                
                if (currentHealthBackingField.Value == 0)
                {
                    if (OnDepleted != null)
                    {
                        OnDepleted.Invoke();   
                    }
                }
            }
        }
        
        public async UniTask InvincibilityFrames()
        {
            if (InvincibilityFrameDuration == 0f) return;
            
            UseInfinity = true;
            await UniTask.Delay(TimeSpan.FromSeconds(InvincibilityFrameDuration.Value));
            UseInfinity = false;
        }
        
        //[OdinSerialize]
        //public IMod<U16>[] Modifiers { get; [UsedImplicitly] private set; }

        [field:SerializeField] public EventReference<UInt16, UInt16> OnChanged               { get; [UsedImplicitly] private set; }
        [field:SerializeField] public EventReference<UInt16, UInt16> OnDecreased             { get; [UsedImplicitly] private set; }
        [field:SerializeField] public EventReference                 OnDepleted              { get; [UsedImplicitly] private set; }
        [field:SerializeField] public EventReference<UInt16, UInt16> OnIncreased             { get; [UsedImplicitly] private set; }
        
        [field:SerializeField] public EventReference                 OnInvincibilityEnabled  { get; [UsedImplicitly] private set; }
        [field:SerializeField] public EventReference                 OnInvincibilityDisabled { get; [UsedImplicitly] private set; }

        public Bool IsZero => Value == 0;
    }
}
