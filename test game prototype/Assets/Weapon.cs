using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float ammo;
    public float currentAmmo;

    public GameObject projectilePrefab;
    public Transform shootPoint;

    public float projectileSpeed;

    private KeyCode reloadButton = KeyCode.R;

    public float timeBetweenShooting;
    private float shootTimer;

    protected GameObject currentProjectileInstance;

    protected virtual void Update()
    {
        // Decrease the shootTimer
        shootTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && shootTimer <= 0f)
        {
            Shoot();
            // Reset the shootTimer
            shootTimer = timeBetweenShooting;
        }
        if (Input.GetKeyDown(reloadButton))
        {
            reload();
        }
    }

   

    protected virtual void Shoot()
    {
        if (currentAmmo > 0 && projectilePrefab != null && shootPoint != null)
        {
            currentProjectileInstance = Instantiate(projectilePrefab, shootPoint.position, Quaternion.Euler(90f, 0f, 0f) );
            //90 rotation on x axis is forward

            Rigidbody rb = currentProjectileInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = shootPoint.forward * projectileSpeed;
            }
            currentAmmo--;
            DefineProjectile();
        }
    }

    protected abstract void DefineProjectile();
    protected abstract void reload();
}
