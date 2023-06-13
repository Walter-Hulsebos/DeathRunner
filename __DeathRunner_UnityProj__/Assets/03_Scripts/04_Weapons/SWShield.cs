using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DeathRunner.Weapons
{
    [CreateAssetMenu]
    public class SWShield : SideWeapon
    {
        //[SerializeField] private HealthComponent _healthComponent;
        
        public async UniTask Shield(GameObject parent, GameObject shield)
        {
            //_damageable = parent.GetComponent<Damageable.Damageable>();
            
            /*
            _healthComponent.health.isInvulnerable = true;
            shield.SetActive(true);
            
            await UniTask.Delay(TimeSpan.FromSeconds(activeTime));
            
            _healthComponent.health.isInvulnerable = false;
            shield.SetActive(false);
            */
        }
    }
}
