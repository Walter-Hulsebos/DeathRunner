using System.Collections;
using System.Collections.Generic;
using DungeonArchitect.Samples.ShooterGame;
using UnityEngine;

namespace DeathRunner
{
    public class EncounterManagerMain : MonoBehaviour
    {

        [SerializeField] public GameObject mainCam;

        [SerializeField] public GameObject cinematicCam;
        
        // Start is called before the first frame update
        void Start()
        {
            cinematicCam.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
