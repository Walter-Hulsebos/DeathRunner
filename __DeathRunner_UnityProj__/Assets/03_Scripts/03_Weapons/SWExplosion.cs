using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DeathRunner.Weapons
{
    public class SWExplosion : SideWeapon
    {
        private GameObject explosion;
        
        void Start()
        {
            GameObject.FindWithTag("InstantiatePos");
        }

        public override void Activate(GameObject parent)
        {
            Instantiate(explosion, parent.transform.position, quaternion.identity);
        }
    }
}
