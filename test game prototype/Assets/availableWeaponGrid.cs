using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class availableWeaponGrid : MonoBehaviour,IDropHandler
{

    LoadoutManager loadoutManager;

    public GameObject weaponCardPrefab;



    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        weaponCardScript droppedScript = dropped.GetComponent<weaponCardScript>();
        dropped.GetComponent<weaponCardScript>().parentTransform = this.transform;
        LoadoutManager.Instance.removeEquippedWeapon(droppedScript.representedWeapon);
    }

    // Start is called before the first frame update
    void Start()
    {
        loadoutManager = LoadoutManager.Instance;

        foreach (var weapon in loadoutManager.availableWeapons)
        {
            GameObject cardInstance = Instantiate(weaponCardPrefab, this.transform);
            cardInstance.GetComponent<weaponCardScript>().representedWeapon = weapon;
            cardInstance.GetComponent<Image>().color = GetRandomColor();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Color GetRandomColor()
    {
        // Generate random values for RGB channels
        float r = Random.value;
        float g = Random.value;
        float b = Random.value;

        // Create a new color using the random values
        Color randomColor = new Color(r, g, b);

        return randomColor;
    }


}
