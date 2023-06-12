using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeathRunner
{
    public class JudgementCut : MonoBehaviour
    {

        [SerializeField] private GameObject hitBox;

        [SerializeField] private AudioClip startSound;
        [SerializeField] private AudioClip cutSound;

        private AudioSource _audioSource;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(EnableHitbox());
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = startSound;
                _audioSource.Play();
        }

        private IEnumerator EnableHitbox()
        {
            yield return new WaitForSeconds(0.8f);
            hitBox.SetActive(true);
            _audioSource.clip = cutSound;
            _audioSource.Play();
             yield return new WaitForSeconds(2.5f);
            gameObject.SetActive(false);
        }
    }
}
