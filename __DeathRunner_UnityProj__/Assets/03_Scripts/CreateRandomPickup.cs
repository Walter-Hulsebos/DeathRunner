using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class CreateRandomPickup : MonoBehaviour
    {
        [SerializeField] private GameObject[] PickUps;
        
        // Start is called before the first frame update
        void Start()
        {
            Instantiate(PickUps[Random.Range(0, PickUps.Length)], transform.position, Quaternion.identity);
        }
    }
}
