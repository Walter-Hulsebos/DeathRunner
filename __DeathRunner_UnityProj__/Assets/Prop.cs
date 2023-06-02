using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
namespace DeathRunner
{
    public class Prop : MonoBehaviour
    {
        private Rigidbody rb;
        private Transform player;

        [SerializeField] private float shootStrength = 1;
        private void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
            rb = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerHitbox"))
            {
                Vector3 direction = player.position - transform.position;
                rb.AddForce(-direction * shootStrength);
                rb.AddTorque(423, 432,323 );
                print("flying");
            }
        }
    }
}
