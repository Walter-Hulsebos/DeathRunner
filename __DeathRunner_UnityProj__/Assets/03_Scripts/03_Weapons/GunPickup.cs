using System;
using System.Collections;
using System.Collections.Generic;
using DeathRunner.Weapons;
using UnityEngine;

namespace Game
{
    public class GunPickup : MonoBehaviour
    {
        [SerializeField] private SideWeapon gun;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<SideWeaponHolder>().sideWeapon = gun;
                Destroy(gameObject);
            }
        }
    }
}
