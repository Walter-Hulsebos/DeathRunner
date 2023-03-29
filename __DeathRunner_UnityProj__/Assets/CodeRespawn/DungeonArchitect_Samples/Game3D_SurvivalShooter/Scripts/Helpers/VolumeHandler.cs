//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;
using UnityEngine.UI;


namespace DungeonArchitect.Samples.ShooterGame {
	public class VolumeHandler : MonoBehaviour {

		// Use this for initialization
		private void Start () 
		{
			if(GameObject.Find("EffectsSlider"))
			GameObject.Find("EffectsSlider").GetComponent<Slider>().onValueChanged.AddListener(SetVolume);
		}

		private void SetVolume(float volume)
		{
			GetComponent<AudioSource>().volume = volume;
		}

		private void OnDestroy()
		{
			if(GameObject.Find("EffectsSlider"))
			GameObject.Find("EffectsSlider").GetComponent<Slider>().onValueChanged.RemoveListener(SetVolume);
		}
	}
}
