using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponHolderScript : MonoBehaviour
{
    private LoadoutManager loadoutManager;

    // List to store references to all instantiated weapons
    public List<GameObject> instantiatedWeapons = new List<GameObject>();
    private int currentWeaponIndex = 0;
    private void Start()
    {
        loadoutManager = LoadoutManager.Instance;
        Debug.Log(loadoutManager.availableWeapons);
        foreach (Weapon weaponPrefab in loadoutManager.availableWeapons)
        {
            GameObject weaponObject = Instantiate(weaponPrefab.gameObject, transform);
            instantiatedWeapons.Add(weaponObject);

            // Deactivate all instantiated weapons
            if(weaponObject != null)
            {
                weaponObject.SetActive(false);
            }
            
        }
        EnableCurrentWeapon();

    }


    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            if (scroll > 0f)
            {
                // Scroll up: switch to the previous weapon
                SwitchToPreviousWeapon();
            }
            else
            {
                // Scroll down: switch to the next weapon
                SwitchToNextWeapon();
            }
        }
    }
    private void SwitchToPreviousWeapon()
    {
        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = instantiatedWeapons.Count - 1;
        }
        EnableCurrentWeapon();
    }

    private void SwitchToNextWeapon()
    {
        currentWeaponIndex++;
        if (currentWeaponIndex >= instantiatedWeapons.Count)
        {
            currentWeaponIndex = 0;
        }
        EnableCurrentWeapon();
    }

    private void EnableCurrentWeapon()
    {
        // Disable all weapons
        foreach (var weapon in instantiatedWeapons)
        {
            weapon.SetActive(false);
        }

        // Enable the current weapon, or keep all weapons disabled if the current index is 0
        
        instantiatedWeapons[currentWeaponIndex].SetActive(true);
        
    }

    
}
