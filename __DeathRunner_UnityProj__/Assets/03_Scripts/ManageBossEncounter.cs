using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DeathRunner
{
    public class ManageBossEncounter : MonoBehaviour
    {

        [SerializeField] private GameObject bossHealthbar;

        [SerializeField] private GameObject[] doors;
        
        [SerializeField] private GameObject boss;

        [SerializeField] private GameObject bossCutsceneTimeline;
        
        
        // Start is called before the first frame update
        void Start()
        {
            bossHealthbar = GameObject.FindWithTag("BossHealthBar");
            bossHealthbar.SetActive(false);
            
            
            boss.SetActive(false);
            
            bossCutsceneTimeline.SetActive(false);
        }

        // Update is called once per frame
        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                return;
            }
            bossHealthbar.SetActive(true);
            bossCutsceneTimeline.SetActive(true);
            foreach (GameObject door in doors)
            {
                door.SetActive(false);
            }

            boss.SetActive(true);
        }
        }
        
    
}