using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    private static SteamManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (!SteamAPI.Init())
            {
                Debug.LogError("SteamAPI_Init failed. Refer to Valve's documentation or the comment above this line for more information.");
                return;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SteamAPI.Shutdown();
        }
    }

    void Update()
    {
        if (SteamAPI.IsSteamRunning())
        {
            SteamAPI.RunCallbacks();
        }
    }
}
