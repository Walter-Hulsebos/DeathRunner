using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace DeathRunner.Weapons
{
    [CreateAssetMenu]
    public class SWGun : SideWeapon
    {
        [SerializeField] private GameObject bulletPrefab;
        private Camera mainCamera;


        public void Shoot(Transform muzzle, Camera camera)
        {
            GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        }
        
    }
}
