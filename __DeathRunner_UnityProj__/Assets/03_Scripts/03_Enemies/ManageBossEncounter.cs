using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace DeathRunner
{
    public class ManageBossEncounter : MonoBehaviour
    {

         private GameObject bossHealthbar;

         [SerializeField] private GameObject introTimeline;
         
        [SerializeField] private GameObject[] doors;
        
        [SerializeField] private GameObject boss;


        [SerializeField] private GameObject endCam;
        
        // Start is called before the first frame update
        void Start()
        {
            bossHealthbar = GameObject.FindWithTag("BossHealthBar");
            bossHealthbar.SetActive(false);
            
            
            boss.SetActive(false);
        }

        // Update is called once per frame
        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                return;
            }
            bossHealthbar.SetActive(true);
            foreach (GameObject door in doors)
            {
                door.SetActive(false);
            }
            introTimeline.SetActive(true);
            boss.SetActive(true);
        }
        
        }
        
    
}
