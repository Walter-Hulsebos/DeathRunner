//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;
using DungeonArchitect.Navigation;

namespace JackRabbit {
	public class FollowPlayer : MonoBehaviour {
		private DungeonNavAgent agent;
		// Use this for initialization
		private void Start () {
			agent = GetComponent<DungeonNavAgent>();
		}
		
		// Update is called once per frame
		private void FixedUpdate () {
			var player = GameObject.FindGameObjectWithTag(DungeonArchitect.Samples.ShooterGame.GameTags.Player);
			agent.Destination = player.transform.position;
		}
	}
}
