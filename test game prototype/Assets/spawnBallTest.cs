using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Netcode;
using UnityEngine;

public class spawnBallTest : NetworkBehaviour
{

    public GameObject ballPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if(Input.GetKeyDown(KeyCode.F))
        {
            spawnBallServerRpc();
        }
    }

    [ServerRpc]
    public void spawnBallServerRpc()
    {
        GameObject ballInstance = Instantiate(ballPrefab,transform.position,Quaternion.identity);
        ballInstance.GetComponent<NetworkObject>().Spawn();
    }
}
