using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class equippedWeaponsGrid : MonoBehaviour,IDropHandler
{



    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        dropped.GetComponent<weaponCardScript>().parentTransform = this.transform;

        weaponCardScript weaponCard = dropped.GetComponent<weaponCardScript>();

        LoadoutManager.Instance.equipWeapon(weaponCard.representedWeapon);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
