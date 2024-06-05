using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Steamworks;

public class NetworkUI : MonoBehaviour
{
    public Button startHost;
    public Button startClient;
    public GameObject UICamera;

    private void Start()
    {
        // Ensure Steam API is initialized
        if (!SteamAPI.IsSteamRunning())
        {
            Debug.LogError("Steam is not running!");
            return;
        }

        if (!SteamAPI.Init())
        {
            Debug.LogError("Failed to initialize Steam API!");
            return;
        }

        startHost.onClick.AddListener(StartHostButtonClick);
        startClient.onClick.AddListener(StartClientButtonClick);
    }

    public void StartHostButtonClick()
    {
        StartHost();
        DestroySelf();
    }

    public void StartClientButtonClick()
    {
        StartClient();
        DestroySelf();
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    void DestroySelf()
    {
        Destroy(gameObject);
        Destroy(UICamera.gameObject);
    }
}
