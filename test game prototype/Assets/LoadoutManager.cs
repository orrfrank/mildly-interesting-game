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

    

    
}
