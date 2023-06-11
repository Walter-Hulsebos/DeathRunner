using UnityEngine;

public class SimpleLookat : MonoBehaviour
{
    // The target to look at.
    [SerializeField] private Transform target;
    
    // The parent of this object.
    [SerializeField] private Transform parent;
    
    // The position offset from the parent.
    [SerializeField] private Vector3 offsetFromParent;
    
    // The rotation offset from the target.
    [SerializeField] private Vector3 offsetRotation;

    // Get the position of the target, but with the y value of this object.
    private Vector3 FlatTargetPosition => new(target.position.x, transform.position.y, target.position.z);

	
    // Called when the script is loaded (Called in the editor only).
	// Finding the target in here ensures that we'll have a reference to the camera immediately when the script is added.
    private void Reset()
    {
        FindTarget();
    }
    
    // Called when a value is changed in the inspector (Called in the editor only).
    private void OnValidate()
    {
        // Set the local position, if the game is playing.
        if (Application.isPlaying)
        {
            //transform.SetParent(parent);
            //transform.localPosition = offsetFromParent;
        }

        // If target is null, find the main camera.
        if (target == null)
        {
            FindTarget();
        }
    }
    
    // Find the main camera and set it as the target.
    private void FindTarget()
    {
        target = Camera.main.transform;
    }
    
    private void Start()
    {
        // Set position relative to parent.
        transform.position = parent.TransformPoint(offsetFromParent);
        //transform.position = parent.position + offsetFromParent;

        //transform.SetParent(parent);
        //transform.localPosition = offsetFromParent;
    }
    
    private void Update()
    {
        // Rotate to look at the target and apply rotation offset.
        transform.LookAt(FlatTargetPosition, Vector3.up);
        transform.Rotate(offsetRotation);
    }
}