using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace DeathRunner.Weapons
{
    [CreateAssetMenu]
    public class SWCut : SideWeapon
    {
        [SerializeField] private GameObject cutPrefab;
        private Camera mainCamera;


        public void SpawnCut(Transform player)
        {
            GameObject cut = Instantiate(cutPrefab, player.position, player.rotation);
        }
        
    }
}