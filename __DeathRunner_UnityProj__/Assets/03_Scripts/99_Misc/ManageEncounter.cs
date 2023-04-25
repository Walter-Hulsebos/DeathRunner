using UnityEngine;

namespace Game
{
    public class ManageEncounter : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private GameObject[] doors;

        private BoxCollider _collider;
        private int deadEnemies = 0;
        
        
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

            _collider = GetComponent<BoxCollider>();
        }

        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {

                foreach( GameObject enemy in enemies )
                {
                    enemy.SetActive(true);
                }
                foreach( GameObject door in doors )
                {
                    door.SetActive(true);
                }

                _collider.enabled = false;
            }
        }
        public void EnemyDied()
        {
            deadEnemies++;
            
            if (deadEnemies == enemies.Length)
            {
                foreach( GameObject door in doors )
                {
                    door.SetActive(false);
                }
            }
        }
    }
    

}
