using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    public Transform pointA, pointB;
    public float speed = 1.0f;
    private float timeCounter = 0f;

    public Rigidbody rb;
    void Update()
    {
        // Increment the time counter based on speed
        timeCounter += Time.deltaTime * speed;

        // Calculate the position along the sine wave
        float x = Mathf.Lerp(pointA.position.x, pointB.position.x, Mathf.Sin(timeCounter));
        float y = Mathf.Lerp(pointA.position.y, pointB.position.y, Mathf.Sin(timeCounter));
        float z = Mathf.Lerp(pointA.position.z, pointB.position.z, Mathf.Sin(timeCounter));

        // Update the platform's position
        rb.MovePosition( new Vector3(x, y, z));
    }
}
