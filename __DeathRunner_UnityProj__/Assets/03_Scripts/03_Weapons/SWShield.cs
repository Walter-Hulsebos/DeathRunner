using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using DeathRunner.Damageable;

namespace DeathRunner.Weapons
{
    [CreateAssetMenu]
    
    public class SWShield : SideWeapon
    {
        private Damageable.Damageable _damageable;
        public async UniTask Shield(GameObject parent, GameObject shield)
        {
            _damageable = parent.GetComponent<Damageable.Damageable>();
            _damageable.isInvulnerable = true;
            shield.SetActive(true);

            await UniTask.Delay(TimeSpan.FromSeconds(activeTime));
            _damageable.isInvulnerable = false;
            shield.SetActive(false);
        }
    }
}
