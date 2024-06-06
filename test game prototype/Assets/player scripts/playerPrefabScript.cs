using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerPrefabScript : MonoBehaviour
{

    public GameObject[] childObjects;
    // Start is called before the first frame update
    private void Awake()
    {
        
        foreach (var obj in childObjects)
        {
            obj.transform.SetParent(null);
        }
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
