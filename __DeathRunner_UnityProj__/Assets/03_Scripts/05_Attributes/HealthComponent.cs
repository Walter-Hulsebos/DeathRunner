using System;
using Sirenix.OdinInspector;
using UnityEngine;

using I32 = System.Int32;
using U16 = System.UInt16;


namespace DeathRunner.Attributes
{
    public sealed class HealthComponent : MonoBehaviour
    {
        public Health health;
        
        private void Awake()
        {
            health.Init(owner: this);
        }
        
        #if ODIN_INSPECTOR
        [Button]
        #endif
        private void DoDamage(U16 damage)
        {
            I32 __newHealth = (health.Value - damage);
            health.Value = (U16)Unity.Mathematics.math.clamp(__newHealth, 0, health.Max.Value);
        }
        
        #if ODIN_INSPECTOR
        [Button]
        #endif
        private void Heal(U16 heal)
        {
            I32 __newHealth = (health.Value + heal);
            health.Value = (U16)Unity.Mathematics.math.clamp(__newHealth, 0, health.Max.Value);
        }
    }
}