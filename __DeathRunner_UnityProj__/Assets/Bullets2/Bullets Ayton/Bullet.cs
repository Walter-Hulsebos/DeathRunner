using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    public GameObject ball;
    [FormerlySerializedAs("launchVelocity")] public float launchSpeed;
    public float destroyTime;
    private Rigidbody rb;

    private void Start()
    {
        rb = ball.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.forward * launchSpeed * Time.deltaTime);
        Destroy(gameObject, destroyTime);
    }
}
