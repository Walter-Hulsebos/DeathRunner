using System;
using Sirenix.OdinInspector;
using UnityEngine;

using I32 = System.Int32;
using U16 = System.UInt16;
using F32 = System.Single;


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
        private void DoDamage(F32 damage)
        {
            health.Value -= damage;
        }
        
        #if ODIN_INSPECTOR
        [Button]
        #endif
        private void Heal(F32 heal)
        {
            health.Value += heal;
        }
    }
}