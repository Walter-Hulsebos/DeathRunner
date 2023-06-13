using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DeathRunner
{
    public class JudgementCut : MonoBehaviour
    {

        [SerializeField] private GameObject hitBox;

        [SerializeField] private AudioClip soundEffectStart;

        [SerializeField] private AudioClip soundEffectCut;

        private AudioSource _audioSource;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(EnableHitbox());
            _audioSource = GetComponent<AudioSource>();

            _audioSource.clip = soundEffectStart;
            _audioSource.Play();
        }

        private IEnumerator EnableHitbox()
        {
            yield return new WaitForSeconds(0.8f);
            hitBox.SetActive(true);
            _audioSource.clip = soundEffectCut;
            _audioSource.Play();
             yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
    }
}
