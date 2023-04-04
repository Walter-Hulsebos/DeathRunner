using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeathRunner.Weapons
{
    public class SideWeapon : ScriptableObject
    {
        public new string name;

        public float cooldownTime;
        
        public float activeTime;
        
        // Start is called before the first frame update
        public virtual void Activate(GameObject gameObject) 
        {
            
        }
    }
}
