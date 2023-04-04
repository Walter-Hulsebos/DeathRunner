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
        
        public override void Activate(GameObject parent)
        {
            //Todo anything but this
            Transform muzzle = parent.transform.Find("Muzzle");
            
            Instantiate(bulletPrefab, muzzle.position ,muzzle.rotation);
        }
    }
}
