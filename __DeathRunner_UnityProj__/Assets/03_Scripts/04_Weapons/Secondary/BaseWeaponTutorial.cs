// using System;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public abstract class BaseWeaponTutorial : MonoBehaviour
// {
//     [SerializeField] private Transform muzzle;
//
//     private float _nextTimeToFire = 0f;
//
//     [SerializeField] 
//     protected WeaponSO weaponSO;
//
//     //This code is for the new InputSystem
//     /*
//     private LookAround actions;
//
//     public void Start()
//     {
//         actions = transform.root.GetComponent<LookAround>();
//     }
//     */
//     private LookWithMouse _actions;
//     private PlayerMovement _inputActions;
//
//     private InputAction _isShooting;
//
//     private void Reset()
//     {
//         muzzle = transform.Find("Muzzle");
//
//         if (muzzle == null)
//         {
//             muzzle = new GameObject(name: "Muzzle").transform;
//             muzzle.parent = transform;
//         }
//     }
//
//     public void Start()
//     {
//         _actions = transform.root.GetComponent<LookWithMouse>();
//         _inputActions = transform.root.GetComponent<PlayerMovement>();
//     }
//
//     private bool CheckFireRate()
//     {
//         if (Time.time >= _nextTimeToFire)
//         {
//             _nextTimeToFire = Time.time + (1f / weaponSO.fireRate);
//             return true;
//         }
//
//         return false;
//     }
//
//     public void Update()
//     {
//         if (_inputActions.isShooting)
//         {
//             if (weaponSO.shotType == WeaponSO.ShotTypeTutorial.Auto)
//             {
//                 if (CheckFireRate())
//                 {
//                     InstanceBullet(muzzle);
//                 }
//             }
//             else if (weaponSO.shotType == WeaponSO.ShotTypeTutorial.Single)
//             {
//                 InstanceBullet(muzzle);
//                 _inputActions.isShooting = false;
//             }
//         }
//     }
//
//     public GameObject InstanceBullet(Transform origin)
//     {
//         GameObject __projectile = Instantiate(
//             weaponSO.bulletPrefab,
//             origin.position,
//             origin.rotation,
//             null
//         );
//         return __projectile;
//     }
//
//     public abstract void Shoot();
// }
