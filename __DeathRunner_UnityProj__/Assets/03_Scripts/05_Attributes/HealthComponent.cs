using UnityEngine;

namespace DeathRunner.Attributes
{
    public sealed class HealthComponent : MonoBehaviour
    {
        public Health health;
        
        private void Awake()
        {
            health.Init(owner: this);
        }
    }
}