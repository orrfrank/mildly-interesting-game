using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocketLauncherScript : Weapon
{
    public float explosionForce;

    public float explosionRadius;

    public GameObject rocketInstance; //the instance of the projectile
    public rocketScript rocketInstanceScript;

    // when instantiating the projectile, access its rocketScript component and apply the options

    protected override void DefineProjectile()
    {
        rocketInstance = currentProjectileInstance;

        rocketInstanceScript = rocketInstance.GetComponent<rocketScript>();


        rocketInstanceScript.explosionRadius = explosionRadius;
        rocketInstanceScript.explosionForce = explosionForce;

    }

    protected override void reload()
    {
        currentAmmo = ammo;
    }

}
