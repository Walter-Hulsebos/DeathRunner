using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Damageable
{
    public class Bullet : MonoBehaviour
    {
        public GameObject ball;

        [FormerlySerializedAs("launchVelocity")]
        public float launchSpeed;

        public float destroyTime;
        private Rigidbody rb;

        private void Start()
        {
            rb = ball.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + transform.forward * launchSpeed * Time.deltaTime);
            Destroy(gameObject, destroyTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Destroy(gameObject);
            }
        //TODO make bullet dissapear when it hits anything except an enemy
        }
    }
}