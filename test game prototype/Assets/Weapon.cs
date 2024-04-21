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

    public Vector3 shootDirection;

    protected virtual void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit,Mathf.Infinity))
        {
            shootDirection = (hit.point - shootPoint.position).normalized;
        }
        else
        {
            shootDirection = shootPoint.forward;
        }

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
                rb.velocity = shootDirection * projectileSpeed;
            }
            currentAmmo--;
            DefineProjectile();
        }
    }

    protected abstract void DefineProjectile();
    protected abstract void reload();
}
