using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocketScript : explosive
{
    
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("Player"))
        {
            explode();
        }
       
    }



    

    private void FixedUpdate()
    {
        // Get the velocity vector
        Vector3 velocity = rb.velocity;

        // Check if the velocity is not zero to avoid division by zero
        if (velocity != Vector3.zero)
        {
            // Calculate the rotation towards the velocity
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);

            // Apply the offset rotation
            Quaternion offsetRotation = Quaternion.Euler(90, 0f, 0f);

            // Combine the rotations
            Quaternion finalRotation = targetRotation * offsetRotation;

            // Apply the rotation
            transform.rotation = finalRotation;
        }
    }
}