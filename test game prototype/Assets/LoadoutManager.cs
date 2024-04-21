using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadoutManager : MonoBehaviour
{
    // Singleton instance
    private static LoadoutManager instance;

    // List of available weapons
    public List<Weapon> availableWeapons = new List<Weapon>();

    // List of currently equipped weapons
    private List<Weapon> equippedWeapons = new List<Weapon>();

    // Public accessor for the singleton instance
    public static LoadoutManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LoadoutManager>();

                // If no instance exists in the scene, create a new GameObject with the LoadoutManager attached
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("LoadoutManager");
                    instance = singletonObject.AddComponent<LoadoutManager>();
                }
            }
            return instance;
        }
    }

    // Method to add a weapon to the available weapons list
    public void AddWeapon(Weapon weapon)
    {
        availableWeapons.Add(weapon);
    }

    // Method to remove a weapon from the available weapons list
    public void RemoveWeapon(Weapon weapon)
    {
        availableWeapons.Remove(weapon);
    }

    // Method to equip a weapon from the available weapons list
    public void EquipWeapon(Weapon weapon)
    {
        // Check if the weapon is available
        if (availableWeapons.Contains(weapon))
        {
            // Equip the weapon
            equippedWeapons.Add(weapon);
        }
        else
        {
            Debug.LogWarning("Trying to equip a weapon that is not available.");
        }
    }

    // Method to unequip a weapon
    public void UnequipWeapon(Weapon weapon)
    {
        if (equippedWeapons.Contains(weapon))
        {
            equippedWeapons.Remove(weapon);
        }
    }

    // Method to switch to a specific weapon
    public void SwitchWeapon(Weapon newWeapon)
    {
        // Unequip all currently equipped weapons
        foreach (var weapon in equippedWeapons)
        {
            weapon.gameObject.SetActive(false);
        }

        // Equip the new weapon
        EquipWeapon(newWeapon);

        // Activate the new weapon
        newWeapon.gameObject.SetActive(true);
    }

    // Method to switch to the next weapon in the loadout
    public void SwitchToNextWeapon()
    {
        // Determine the index of the next weapon in the loadout
        int currentIndex = equippedWeapons.IndexOf(equippedWeapons.Find(w => w.gameObject.activeSelf));
        int nextIndex = (currentIndex + 1) % equippedWeapons.Count;

        // Switch to the next weapon
        SwitchWeapon(equippedWeapons[nextIndex]);
        Debug.Log(GetCurrentWeapon());
    }

    // Method to switch to the previous weapon in the loadout
    public void SwitchToPreviousWeapon()
    {
        // Determine the index of the previous weapon in the loadout
        int currentIndex = equippedWeapons.IndexOf(equippedWeapons.Find(w => w.gameObject.activeSelf));
        int previousIndex = (currentIndex - 1 + equippedWeapons.Count) % equippedWeapons.Count;

        // Log the current equipped weapon before switching
        Debug.Log(GetCurrentWeapon());

        // Switch to the previous weapon
        SwitchWeapon(equippedWeapons[previousIndex]);
    }

    public Weapon GetCurrentWeapon()
    {
        return equippedWeapons.FirstOrDefault(w => w.gameObject.activeSelf);
    }

}
