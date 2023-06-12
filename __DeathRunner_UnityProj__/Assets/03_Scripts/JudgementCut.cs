using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeathRunner
{
    public class JudgementCut : MonoBehaviour
    {

        [SerializeField] private GameObject hitBox;
            
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(EnableHitbox());
        }

        private IEnumerator EnableHitbox()
        {
            yield return new WaitForSeconds(0.8f);
            hitBox.SetActive(true);
            // yield return new WaitForSeconds(0.1f);
            // hitBox.SetActive(false);
        }
    }
}
