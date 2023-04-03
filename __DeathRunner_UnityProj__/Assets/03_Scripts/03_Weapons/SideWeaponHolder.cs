using System;
using System.Collections;
using System.Collections.Generic;
using DG.DemiEditor.DeGUINodeSystem;
using UnityEngine;

namespace DeathRunner.Weapons
{
    public class SideWeaponHolder : MonoBehaviour
    {

        public SideWeapon sideWeapon;

        private float cooldownTime;

        private float activeTime;
        
        // Update is called once per frame
        enum WeaponState
        {
         ready,
         active,
         cooldown
        } 

        private WeaponState state = WeaponState.ready;

        private void Update()
        {
            switch (state)
            {
                case WeaponState.ready:
                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        sideWeapon.Activate(gameObject);
                        state = WeaponState.active; 
                        activeTime = SideWeapon.activeTime;
                    }
                    break;
                case WeaponState.active:
                    if (activeTime > 0)
                    {
                        activeTime -= Time.deltaTime;
                    }
                    else
                    {
                        state = WeaponState.active;
                        cooldownTime = sideWeapon.cooldownTime;
                    }
                    break;
                
                case WeaponState.cooldown:
                    if (cooldownTime > 0)
                    {
                        cooldownTime -= Time.deltaTime;
                    }
                    else
                    {
                        state = WeaponState.ready;
                    }
                    break;
            }
      
        }
    }
}
