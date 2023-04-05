using System;
using System.Collections;
using System.Collections.Generic;
//using DeathRunner.Movement;
using DG.DemiEditor.DeGUINodeSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace DeathRunner.Weapons
{
    public class SideWeaponHolder : MonoBehaviour
    {

        public SideWeapon sideWeapon;

        private float cooldownTime;

        private float activeTime;

        [SerializeField] Image cooldownImage;

        [SerializeField] private Transform muzzle;

        [SerializeField] private InputActionReference secondaryFire;
        
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
                    if (secondaryFire.action.IsPressed())
                    {
                        if (sideWeapon is SWGun gun)
                        {
                            gun.Shoot(muzzle);
                        }
                        else
                        {
                            sideWeapon.Activate(gameObject);    
                        }
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
                        cooldownImage.fillAmount = 1 - (cooldownTime / sideWeapon.cooldownTime);
                    }
                    else
                    {
                        cooldownImage.fillAmount = 1;
                        state = WeaponState.Ready;
                    }
                    break;
            }
      
        }
    }
}
