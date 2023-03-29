//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

public class PlayerCameraZoom : MonoBehaviour {
	public float sensitivity = 1;
	public float zoomMultiplier = 1.2f;
	public float maxSpeed = 8;
	public Rigidbody2D rigidBody2D;

	private float startingZoom;
	private float targetZoom;
	private Camera cam;

	private void Awake() {
		cam = GetComponent<Camera>();
		startingZoom = cam.orthographicSize;
	}

	// Update is called once per frame
	private void Update () {
		var speed = rigidBody2D.velocity.magnitude;
		var t = speed / maxSpeed;
		var multiplier = Mathf.Lerp (1, zoomMultiplier, t);
		targetZoom = startingZoom * multiplier;

		cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, sensitivity * Time.deltaTime);
	}
}
