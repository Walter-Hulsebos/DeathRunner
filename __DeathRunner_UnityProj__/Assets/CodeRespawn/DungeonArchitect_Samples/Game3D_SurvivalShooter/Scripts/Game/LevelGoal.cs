//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace DungeonArchitect.Samples.ShooterGame {
    public class LevelGoal : MonoBehaviour {
	    private void OnTriggerEnter(Collider other) {
			// Create a new level
			if (other.gameObject.CompareTag(GameTags.Player)) {
				GameController.Instance.CreateNewLevel();
			}
		}

	    private void Update() {
			if (Input.GetKeyDown(KeyCode.Space)) {
				GameController.Instance.CreateNewLevel();
			}
		}
	}
}
