 using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class cameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("input")]
    public Transform orientation;

    //camera
    float mouseX;
    float mouseY;

    float rotationX;
    float rotationY;

    public float rotationZ;


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
        mouseX = Input.GetAxis("Mouse X") * sensitivity ;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity ;

    }


    void rotateCamera()
    {
        //handles vertical rotation
        rotationX -= mouseY;
        rotationY += mouseX;

        rotationX = Mathf.Clamp(rotationX, -90, 90);

        // Apply the vertical rotation to the camera
        transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);



        if (orientation != null)
        {
            orientation.transform.rotation = Quaternion.Euler(0, rotationY, 0f);
        }
       
    }




}
