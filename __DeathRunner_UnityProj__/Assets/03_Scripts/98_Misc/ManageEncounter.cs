using System;
using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using UnityEngine;

namespace Game
{
    public class ManageEncounter : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private GameObject[] doors;

        private int deadEnemies = 0;

        private bool isInCombat = false;
        
        // Start is called before the first frame update
        void Start()
        {
            foreach( GameObject enemy in enemies )
            {
                enemy.SetActive(false);
            }
            foreach( GameObject door in doors )
            {
                door.SetActive(false);
            }
        }

        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isInCombat = true;
                
                foreach( GameObject enemy in enemies )
                {
                    enemy.SetActive(true);
                }
                foreach( GameObject door in doors )
                {
                    door.SetActive(true);
                }
            }
        }

        private void Update()
        {
            //TOdo dont do this in update, call it from the enemy.
            if (!isInCombat) {return;}
            
            foreach( GameObject enemy in enemies )
            {
                if (enemy == null)
                {
                    deadEnemies++;
                }
            }

            if (deadEnemies == enemies.Length)
            {
                foreach( GameObject door in doors )
                {
                    door.SetActive(false);
                }
            }
            
            deadEnemies = 0;
        }
    }
}
