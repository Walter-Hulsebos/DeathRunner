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
                __health.health.Value -= damageInflicted;
            }
        }
    }
}