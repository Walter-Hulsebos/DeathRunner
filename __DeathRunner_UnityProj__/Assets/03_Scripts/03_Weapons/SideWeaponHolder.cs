using System;
using System.Collections;
using System.Collections.Generic;
//using DeathRunner.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace DeathRunner.Weapons
{
    public class SideWeaponHolder : MonoBehaviour
    {

        public SideWeapon sideWeapon;

        [Space]
        private float cooldownTime;

        private float activeTime;

        [SerializeField] Image cooldownImage;

        [SerializeField] private Transform muzzle;
        
        [SerializeField] GameObject shieldObj;

        [SerializeField] private InputActionReference secondaryFire;

        [SerializeField] private Camera _mainCamera;
        
        // Update is called once per frame
        enum WeaponState
        {
         Ready,
         Active,
         Cooldown
        }

        private void Start()
        {
            shieldObj.SetActive(false);
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
                            gun.Shoot(muzzle, _mainCamera);
                        }
                        else if (sideWeapon is SWShield shield)
                        {
                            shield.Shield(gameObject, shieldObj);    
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
