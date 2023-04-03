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
         Ready,
         Active,
         Cooldown
        } 

        private WeaponState state = WeaponState.Ready;

        private void Update()
        {
            switch (state)
            {
                case WeaponState.Ready:
                    //TODO use input system
                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        sideWeapon.Activate(gameObject);
                        state = WeaponState.Active; 
                        activeTime = sideWeapon.activeTime;
                    }
                    break;
                case WeaponState.Active:
                    Debug.Log("Active");
                    if (activeTime > 0)
                    {
                        activeTime -= Time.deltaTime;
                    }
                    else
                    {
                        state = WeaponState.Cooldown;
                        cooldownTime = sideWeapon.cooldownTime;
                    }
                    break;
                
                case WeaponState.Cooldown:
                    Debug.Log("Cooldown");
                    if (cooldownTime > 0)
                    {
                        cooldownTime -= Time.deltaTime;
                    }
                    else
                    {
                        state = WeaponState.Ready;
                    }
                    break;
            }
      
        }
    }
}
