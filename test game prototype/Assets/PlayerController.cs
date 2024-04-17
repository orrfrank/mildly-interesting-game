using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{

    //states
    private PlayerStates currentState;

    private AirborneStates currentAirState;

    [Header("references")]
    public Transform headLocation;
    Rigidbody rb;

    cameraScript camScript;
    

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

    public float fallMultiplier;
    public float lowFallMultiplier;

    [Header("grounded movement settings")]
    public bool isGrounded;

    public float groundDrag;

    RaycastHit slopeNormal;

    public LayerMask groundLayer;


    [Header("Jump Settings")]
    public float jumpForce;
    public float wallJumpForce;
    private bool initiateJump;
    [Header("wallrun settings")]

    private RaycastHit wallHit;
    public float cameraPitchSpeed = 0.1f;
    public float wallrunCameraDegrees;
    public float wallrunGravity;
   

    public enum PlayerStates
    {
        grounded,
        airborne,
        inACutscene,

    }

    public enum AirborneStates
    {
        notAirborne,
        notWallrunning,
        wallRunningLeft,
        wallRunningRight,
    }

    private void TransitionToState(PlayerStates newState)
    {
        currentState = newState;
    }
    private void TransitionToAirState(AirborneStates newState)
    {
        currentAirState = newState;
    }

    bool onSlope()
    {
        
        if(Physics.Raycast(transform.position,-transform.up, out slopeNormal,transform.localScale.y + 0.3f,groundLayer))
        {
            if (slopeNormal.normal != transform.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    // Start is called before the first frame update
    void getComponents()
    {
        rb = GetComponent<Rigidbody>();
        camScript = Camera.main.GetComponent<cameraScript>();
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

        Application.targetFrameRate = 144;
        
    }

    void linearJumpLogic()
    {
        if(rb.velocity.y < 0)
        {
            rb.velocity += transform.up * -(fallMultiplier - 1) * Time.deltaTime;
        }
        else if(rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += transform.up * -(lowFallMultiplier - 1) * Time.deltaTime;
        }
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
                isGrounded = true;
                groundedLogic();
                resetCameraPitch();
                break;
            case PlayerStates.airborne:
                isGrounded = false;
                

                

                //detect wallrunning

                if(Physics.Raycast(transform.position,transform.right,out wallHit,0.7f))
                {
                    TransitionToAirState(AirborneStates.wallRunningRight);
                }
                else if(Physics.Raycast(transform.position, -transform.right,out wallHit, 0.7f))
                {
                    TransitionToAirState(AirborneStates.wallRunningLeft);
                }
                else
                {
                    TransitionToAirState(AirborneStates.notWallrunning);
                }
                airborneStateLogic();
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

                if (onSlope())
                {
                    rb.AddForce(Vector3.ProjectOnPlane(moveDirection, slopeNormal.normal) * (speed + sprintingBonus), ForceMode.Force);
                    
                }
                else
                {
                    rb.AddForce(moveDirection * (speed + sprintingBonus), ForceMode.Force);
                }
                


                
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

    void airborneStateLogic()
    {
        switch (currentAirState)
        {
            case AirborneStates.notAirborne:
                break;
            case AirborneStates.notWallrunning:
                linearJumpLogic();
                resetCameraPitch();
                useNormalGravity();
                Debug.Log("linear");
                break;

            case AirborneStates.wallRunningLeft:
                // Add logic specific to wall running left
                Debug.Log("wallrunning left");
                if(initiateJump)
                {
                    jumpFromWall(-wallHit.normal.normalized);
                }
                pitchCamera(-1);

                useWallrunningGravity();
                break;

            case AirborneStates.wallRunningRight:
                // Add logic specific to wall running right
                Debug.Log("wallrunning right");
                if (initiateJump)
                {
                    jumpFromWall(-wallHit.normal.normalized);
                }
                pitchCamera(1);

                useWallrunningGravity();
                break;
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
            TransitionToAirState(AirborneStates.notAirborne);
        }
        else
        {
            TransitionToState(PlayerStates.airborne);
            TransitionToAirState(AirborneStates.notWallrunning);
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

    void jumpFromWall(Vector3 wallDirection)
    {

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        initiateJump = false;
        rb.AddForce((transform.up * jumpForce) + -wallDirection * wallJumpForce, ForceMode.Impulse);
    }

    void useWallrunningGravity()
    {
        if(rb.velocity.y < 0)
        {
            rb.useGravity = false;
            rb.velocity += -transform.up * wallrunGravity * Time.deltaTime;
        }
        else
        {
            useNormalGravity();
        }
       
    }
    void useNormalGravity()
    {
        rb.useGravity = true;
    }
    void pitchCamera(float direction)
    {
        camScript.rotationZ = Mathf.Lerp(camScript.rotationZ, wallrunCameraDegrees * direction, cameraPitchSpeed * Time.deltaTime);
    }

    void resetCameraPitch()
    {
        // Reset the camera rotation to its initial state
        camScript.rotationZ = Mathf.Lerp(camScript.rotationZ, 0,  (wallrunCameraDegrees/ cameraPitchSpeed) * Time.deltaTime);
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
