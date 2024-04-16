using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocketScript : MonoBehaviour
{
    public float explosionRadius;
    public float explosionForce;

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



    void explode()
    {
        foreach (var rb in nearbyRigidbodies())
        {
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }


        Destroy(this.gameObject);
    }


    Rigidbody[] nearbyRigidbodies()
    {
        // Get colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // Initialize a list to store nearby Rigidbody components
        List<Rigidbody> nearbyRigidbodies = new List<Rigidbody>();

        // Loop through all colliders and add Rigidbody components to the list
        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                nearbyRigidbodies.Add(rb);
            }
        }

        // Convert the list to an array and return
        return nearbyRigidbodies.ToArray();
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