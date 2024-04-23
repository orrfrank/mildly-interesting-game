using UnityEngine;

public class StickyGrenade : explosive
{
    // Flag to check if the grenade is stuck to something
    public bool isStuck = false;

    // Reference to the Rigidbody component of the grenade
    private Rigidbody rb;

    // Reference to the Collider component of the grenade
    private Collider col;

    void Start()
    {
        // Get references to the Rigidbody and Collider components
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the grenade hasn't already stuck to something and if it collides with another object
        if (!isStuck && collision.gameObject.tag != "Player")
        {
            // Stick the grenade to the collided object
            StickToSurface(collision.contacts[0].point, collision.contacts[0].normal);
        }
    }

    // Function to stick the grenade to a surface
    void StickToSurface(Vector3 point, Vector3 normal)
    {
        // Freeze the grenade's rotation and disable its Rigidbody
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.isKinematic = true;

        // Set the position and rotation of the grenade to match the surface it's stuck to
        transform.position = point;
        transform.rotation = Quaternion.LookRotation(normal);

        // Disable the Collider to prevent further collisions
        col.enabled = false;

        // Set the flag to indicate that the grenade is now stuck
        isStuck = true;
    }


    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            explode();
        }
    }
}
