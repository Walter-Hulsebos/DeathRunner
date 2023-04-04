using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DeathRunner.Weapons
{
    [CreateAssetMenu]
    public class SWExplosion : SideWeapon
    {
        public GameObject explosion;
        

        public override void Activate(GameObject parent)
        {
            Instantiate(explosion, parent.transform.position, quaternion.identity);
        }
    }
}
