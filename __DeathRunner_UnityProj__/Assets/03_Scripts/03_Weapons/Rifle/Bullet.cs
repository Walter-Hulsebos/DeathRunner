using UnityEngine;

namespace Damageable
{
    public class Bullet : MonoBehaviour
    {
        public GameObject ball;
        
        public float launchSpeed = 20;

        public float destroyTime = 5;

        //[SerializeField] private LayerMask collideWith;

        
        [SerializeField] private string hitTag = "Player";
        
        //public Collider ignorePhysicsWith;
        
        [SerializeField] private Rigidbody rb;

        private void Reset()
        {
            FindRigidbody();
        }

        [ContextMenu("FindRigidbody")]
        private void FindRigidbody()
        {
            rb = ball.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            rb.MovePosition(rb.position + transform.forward * launchSpeed * Time.deltaTime);
            Destroy(this.gameObject, t: destroyTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(hitTag))
            {
                Destroy(this.gameObject);
            }
            
            //TODO make bullet dissapear when it hits anything except an enemy
        }
    }
}