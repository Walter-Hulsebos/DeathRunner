using System;
using UnityEngine;

using F32 = System.Single;

namespace DeathRunner.Attributes
{
    public sealed class HealthDamage : MonoBehaviour
    {
        [SerializeField] private String tagToDamage;
        
        [SerializeField] private F32 damageInflicted = 1;
        
        // [SerializeField] private DamageLogic damageLogic;
        //
        // [SerializeField] private DamageType damageType;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(tagToDamage)) return;

            if (other.TryGetComponent(out HealthComponent __health))
            {
                __health.health.Value -= damageInflicted;
            }
        }
    }
}