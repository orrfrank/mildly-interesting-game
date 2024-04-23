using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosive : MonoBehaviour
{
    public float explosionRadius;
    public float explosionForce;
    protected void explode()
    {
        foreach (var rb in nearbyRigidbodies())
        {
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }
        Debug.Log("exploded");

        Destroy(this.gameObject);
    }



    protected Rigidbody[] nearbyRigidbodies()
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
}
