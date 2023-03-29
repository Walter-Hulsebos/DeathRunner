//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public float movementSpeed = 15;

	// Use this for initialization
	private void Start () {
	
	}
	
	// Update is called once per frame
	private void Update () {
		float forward = Input.GetAxis ("Vertical"); 
		float right = Input.GetAxis ("Horizontal"); ;
		var distance = movementSpeed * Time.deltaTime;

		// forward movement
		gameObject.transform.position += transform.forward * distance * forward;

		// strafe movement
		gameObject.transform.position += transform.right * distance * right;
	}
}
