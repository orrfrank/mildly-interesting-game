using System.Collections;
using System.Collections.Generic;
using Unity.Physics;
using UnityEngine;

public class GrenadeLauncher : Weapon
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    

    protected override void reload()
    {
        currentAmmo = ammo;
    }

    protected override void DefineProjectile()
    {
        //no projectile yet
    }


}
