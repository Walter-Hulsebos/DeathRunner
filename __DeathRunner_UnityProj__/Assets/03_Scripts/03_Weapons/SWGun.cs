using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DeathRunner.Weapons
{
    [CreateAssetMenu]
    public class SWGun : SideWeapon
    {
        [SerializeField] private GameObject bulletPrefab;
        
        public void Shoot(Transform muzzle)
        {
            Instantiate(bulletPrefab, muzzle.position ,muzzle.rotation);
        }
    }
}
