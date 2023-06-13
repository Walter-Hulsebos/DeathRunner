using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DeathRunner
{
    public class AOEAttack : MonoBehaviour
    {
        [SerializeField] private GameObject hitbox;

        [SerializeField] private GameObject healthDrop;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(EnableHitbox());
        }

        // Update is called once per frame
        private IEnumerator EnableHitbox()
        {
            yield return new WaitForSeconds(0.6f);
            hitbox.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            hitbox.SetActive(false);
            Instantiate(healthDrop, transform.position, quaternion.identity);
            yield return new WaitForSeconds(1.5f);
            gameObject.SetActive(false);
        }
    }
}