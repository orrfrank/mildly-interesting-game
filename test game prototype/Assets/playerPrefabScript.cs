using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class playerPrefabScript : NetworkBehaviour
{
    public GameObject cameraPrefab;
    public PlayerController playerController;
    public Transform playerOrientation;

    private void Awake()
    {
        
        GameObject cameraInstance = Instantiate(cameraPrefab);
        playerController.setCamera(cameraInstance);
        cameraInstance.GetComponentInChildren<cameraScript>().setOrientation(playerOrientation);
        if(IsOwner)
        {
            cameraInstance.gameObject.SetActive(false);
        }
    }

}
