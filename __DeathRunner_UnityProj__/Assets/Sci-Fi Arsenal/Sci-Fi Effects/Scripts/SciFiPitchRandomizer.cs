using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace SciFiArsenal
{

	public class SciFiPitchRandomizer : MonoBehaviour
	{
		[SerializeField] private AudioSource audioSource;
		[SerializeField, HideInInspector] private Boolean hasAudioSource;

		public float randomPercent = 10;
		[SerializeField, HideInInspector] private float randomPrimantissa = 0.1f;
	
		#if UNITY_EDITOR
		private void Reset()
		{
			FindAudioSource();
		}

		private void OnValidate()
		{
			FindAudioSource();
			
			randomPrimantissa = randomPercent / 100;
		}
		
		[ContextMenu("Find Audio Source")]
		private void FindAudioSource()
		{
			audioSource = GetComponent<AudioSource>();
			hasAudioSource = audioSource != null;
		}
		#endif

		private void Start()
		{
			//transform.GetComponent<AudioSource>().pitch *= 1 + Random.Range(-randomPercent / 100, randomPercent / 100);

			if (hasAudioSource)
			{
				audioSource.pitch *= 1 + Random.Range(-randomPrimantissa, randomPrimantissa);
			}
		}
	}
}