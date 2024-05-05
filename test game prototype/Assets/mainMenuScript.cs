using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuScript : MonoBehaviour
{
    LoadoutManager loadoutManager;

    // Start is called before the first frame update
    void Start()
    {
        loadoutManager = LoadoutManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Jump"))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }



    public void removeEquippedWeapon(Weapon weapon)
    {
        loadoutManager.equippedWeapons.Remove(weapon);
    }


    public void equipWeapon(Weapon weapon)
    {
        loadoutManager.equippedWeapons.Add(weapon);
    }

    public void clearEquippedWeapons(Weapon weapon)
    {
        loadoutManager.equippedWeapons.Clear();
    }

}
