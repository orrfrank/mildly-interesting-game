using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStates currentState;
    [Header("references")]
    public Transform headLocation;
    Rigidbody rb;

    

    [Header("ground check options")]
    public Transform groundCheckTransform;
    public float groundCheckRadius;

    [Header("inputs")]
    private Vector2 move;
    private Vector3 moveDirection;
    private bool jump;

    public float jumpBufferTimer;

    [Header("speed")]
    float speed;
    public float airSpeed;
    public float groundSpeed;

    public float timeToSprint;
    public float sprintingSpeed;
    private float sprintingBonus;

    public KeyCode sprintKey = KeyCode.LeftShift;

    public float sprintingFov;

    private float cameraFov = 90;


    [Header("drag settings")]
    public float drag;

    [Header("airborne settings")]
    
    public float airDrag;

    [Header("grounded movement settings")]
    public bool isGrounded;

    public float groundDrag;

    public LayerMask groundLayer;


    [Header("Jump Settings")]
    public float jumpForce;
    private bool initiateJump;

    public enum PlayerStates
    {
        grounded,
        airborne,
        inACutscene,

    }
    private void TransitionToState(PlayerStates newState)
    {
        currentState = newState;
    }


    // Start is called before the first frame update
    void getComponents()
    {
        rb = GetComponent<Rigidbody>();
    }
    void gatherInputs()
    {
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveDirection = (transform.forward * move.y + transform.right * move.x).normalized;
        if(Input.GetButtonDown("Jump"))
        {
            initiateJump = true;
            StartCoroutine(bufferJump());
        }



    }

    void Start()
    {
        getComponents();
        


        
    }

    // Update is called once per frame
    void Update()
    {
        checkGrounded();
        gatherInputs();
        updateStateActions();

        sprintingLogic();
    }
    private void FixedUpdate()
    {
        moveCamera();
        dragForces();
        fixedUpdateStateActions();

    }


    void updateStateActions()
    {
        switch (currentState)
        {
            case PlayerStates.grounded:
                
                groundedLogic();
                break;
            case PlayerStates.airborne:
                
                break;
        }


    }
    void fixedUpdateStateActions()
    {
        switch (currentState)
        {
            case PlayerStates.grounded:
                drag = groundDrag;
                speed = groundSpeed;
                rb.AddForce(moveDirection * (speed + sprintingBonus), ForceMode.Force);


                
                break;

            case PlayerStates.airborne:
                speed = airSpeed;
                drag = airDrag;
                rb.AddForce(moveDirection * (speed + sprintingBonus), ForceMode.Force);
                


                break;
        }


    }

    //gets called in update if grounded
    void groundedLogic()
    {

        //handle jump
        if (initiateJump)
        {
            startJump();
        }

        
    }

    void sprintingLogic()
    {
        if (Input.GetKey(sprintKey))
        {
            sprintingBonus = Mathf.Lerp(sprintingBonus, sprintingSpeed, timeToSprint);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, sprintingFov, timeToSprint * 10 * Time.deltaTime);
        }
        else
        {
            sprintingBonus = Mathf.Lerp(sprintingBonus, 0f, timeToSprint);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, cameraFov, timeToSprint * 10 * Time.deltaTime);
        }
    }

    void checkGrounded()
    {
        bool isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            TransitionToState(PlayerStates.grounded);
        }
        else
        {
            TransitionToState(PlayerStates.airborne);
        }
    }

    void startJump()
    {
        rb.velocity =  new Vector3 (rb.velocity.x, 0, rb.velocity.z);
        initiateJump = false;
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    IEnumerator bufferJump()
    {
        yield return new WaitForSeconds(jumpBufferTimer);
        if (!isGrounded)
        {
            initiateJump = false;
        }
    }

    void moveCamera()
    {
        Camera.main.transform.position = headLocation.position;
    }



    void dragForces()
    {
        float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
        Vector3 forwardDragForce = -drag * forwardSpeed * transform.forward;

        // Calculate drag force along the right direction
        float rightSpeed = Vector3.Dot(rb.velocity, transform.right);
        Vector3 rightDragForce = -drag * rightSpeed * transform.right;

        // Apply the drag forces
        rb.AddForce(forwardDragForce, ForceMode.Force);
        rb.AddForce(rightDragForce, ForceMode.Force);
    }




   
}
