using DeathRunner.Weapons;
using UnityEngine;

namespace Game
{
    public class SideWeaponPickup : MonoBehaviour
    {
        [SerializeField] private SideWeapon sideWeapon;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<SideWeaponHolder>().sideWeapon = sideWeapon;
                Destroy(gameObject);
            }
        }
    }
}
