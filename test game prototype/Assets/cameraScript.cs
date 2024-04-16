 using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class cameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("input")]

    //camera
    float mouseX;
    float mouseY;

    float rotationX;
    float rotationY;


    [Header("settings")]

    public float sensitivity;

    [Header("references")]
    GameObject player;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    
    }
    private void Update()
    {
        handleInput();
        rotateCamera();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        
        

    }

    void handleInput()
    {
        mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

    }



    void rotateCamera()
    {

        //handles vertical rotation


        rotationX -= mouseY;
        rotationY += mouseX;

        rotationX = Mathf.Clamp(rotationX, -90, 90);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);

        // Apply the vertical rotation to the camera
        player.transform.localRotation = Quaternion.Euler(0,rotationY, 0f);

    }

    

}
