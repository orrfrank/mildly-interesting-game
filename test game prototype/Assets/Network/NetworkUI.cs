using Steamworks;
using Steamworks.Data;
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

    public GameNetworkManager networkManager;


    private void Awake()
    {
        startHost.onClick.AddListener(() =>
        {
            networkManager.StartHost(2);
        });

        startClient.onClick.AddListener(() =>
        {
            SteamFriends.OpenOverlay("friends");
            
        });

        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
    }
    private void OnLobbyEntered(Lobby lobby)
    {
        DestroySelf();
    }
    void DestroySelf()
    {
        Destroy(this.gameObject);
        Destroy(UICamera.gameObject);
    }

}
