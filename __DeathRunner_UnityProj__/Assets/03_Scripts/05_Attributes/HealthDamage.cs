using System;
using UnityEngine;

namespace DeathRunner.Attributes
{
    public sealed class HealthDamage : MonoBehaviour
    {
        
        [SerializeField] private String tagToDamage;
        
        [SerializeField] private UInt16 damageInflicted = 1;
        
        // [SerializeField] private DamageLogic damageLogic;
        //
        // [SerializeField] private DamageType damageType;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(tagToDamage)) return;
            
            //DamageMessage data;
            //data.amount = damageInflicted;
            //data.damager = this;

            if (other.TryGetComponent(out HealthComponent __health))
            {
                checked
                {
                    //Calculate damage to not go below 0
                    
                    Int32 __healthValue     = __health.health.Value;
                    Int32 __damageToInflict = damageInflicted;
                    
                    Int32 __newHealthValue = __healthValue - __damageToInflict;
                    
                    if (__newHealthValue < 0)
                    {
                        __newHealthValue = 0;
                    }
                    
                    __health.health.Value = (UInt16)__newHealthValue;
                }
            }
        }
    }
}