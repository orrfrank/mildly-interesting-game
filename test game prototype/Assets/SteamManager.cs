using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    private void Awake()
    {
        if (!SteamAPI.Init())
        {
            Debug.LogError("SteamAPI_Init() failed. Steam must be running to play this game (Steamworks.NET)");
            return;
        }
    }

    private void OnDestroy()
    {
        SteamAPI.Shutdown();
    }

    private void Update()
    {
        SteamAPI.RunCallbacks();
    }
}
