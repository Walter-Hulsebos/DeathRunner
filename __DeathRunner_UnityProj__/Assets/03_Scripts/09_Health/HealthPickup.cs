using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using DeathRunner.Damageable;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class HealthPickup : MonoBehaviour
    {
        private bool isReady = false;
        private Rigidbody rb;

        [SerializeField] private BoxCollider _boxCollider;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            StartCoroutine(EnableHealing());

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0, 180), transform.eulerAngles.z);

            gameObject.layer = LayerMask.NameToLayer("Pickup");
            float speed = 200;
            rb.isKinematic = false;
            Vector3 force = transform.forward;
            force = new Vector3(force .x, 1, force .z);
            rb.AddForce(force * speed );
        }
        

        IEnumerator EnableHealing()
        {
            yield return new WaitForSeconds(0.75f);
            gameObject.layer = LayerMask.NameToLayer("Default");
            isReady = true;
        }
        // Start is called before the first frame update
        private void OnTriggerStay(Collider other)
        {
            if (!isReady) {return;}
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Damageable>().HealDamage(2);
                Destroy(gameObject);
            }
        }
    }
}

