using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    public Button startHost;
    public Button startClient;

    public GameObject UICamera;


    private void Awake()
    {
        startHost.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            DestroySelf();
        });

        startClient.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            DestroySelf() ;
        });
    }

    void DestroySelf()
    {
        Destroy(this.gameObject);
        Destroy(UICamera.gameObject);
    }

}
