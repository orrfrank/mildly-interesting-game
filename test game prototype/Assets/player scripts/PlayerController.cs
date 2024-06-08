using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour
{
    
    //states
    private PlayerStates currentState;

    private AirborneStates currentAirState;

    [Header("references")]
    public Transform headLocation;
    public Transform orientation;
    public Transform camHolder;
    Rigidbody rb;

    public cameraScript camScript;
    public Camera cam;
    
    

    [Header("ground check options")]
    public Transform groundCheckTransform;
    public float groundCheckRadius;

    [Header("inputs")]
    private Vector2 move;
    private Vector3 moveDirection;
    private bool jump;

    public float jumpBufferTimer;
    public float rotationSpeed;
    [Header("speed")]
    float speed;
    public float airSpeed;
    public float groundSpeed;

    public float timeToSprint;
    public float sprintingSpeed;
    private float sprintingBonus;

    public KeyCode sprintKey = KeyCode.LeftShift;

    

    private float cameraFov = 90;


    [Header("drag settings")]
    public float airDrag;
    public float groundDrag;
    public float drag { get; private set; }

    [Header("airborne settings")]
    
    

    public float fallMultiplier;
    public float lowFallMultiplier;

    [Header("grounded movement settings")]
    public bool isGrounded;

   

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
    public float wallRunForwardBoost;

    [Header("dash settings")]
    public float dashDistance;
    public float dashForce;
    public float dashCooldown;
    public float dashSlowdown;

    private bool isDashing;
    private bool canDash = true;

    public float dashingFov;

    Coroutine dashCooldownCoroutine;
    private Vector3 velBeforeDash = Vector3.zero;

    [Header("movingPlatforms")]
    public float movingPlatformConstant;
    GameObject currentMovingPlatform;
    Vector3 currentPlatformVel;
    Vector3 previousPlatformVel;
    private Dictionary<GameObject, Vector3> previousPlatformPositions = new Dictionary<GameObject, Vector3>();
    private Vector3 targetVel;
    Vector3 smoothedPlatformVel;
    public float smoothingSpeed;

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
    }
    void gatherInputs()
    {
        Vector3 viewDir = transform.position - new Vector3(cam.transform.position.x, transform.position.y, cam.transform.position.z);
        orientation.forward = viewDir.normalized;

         

        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveDirection = (orientation.forward * move.y + orientation.right * move.x).normalized;
        if(Input.GetButtonDown("Jump"))
        {
            initiateJump = true;
            StartCoroutine(bufferJump());
        }
    }

    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

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

        dashingLogic();


    }

    private void LateUpdate()
    {
        if(camHolder != null)
        {
            //moveCamera();
        }
        
    }

    private void FixedUpdate()
    {

        dragForces();
        fixedUpdateStateActions();
        // Reset platform-related variables if not on a moving platform
        if (currentMovingPlatform == null)
        {
            currentPlatformVel = Vector3.zero;
            previousPlatformVel = Vector3.zero;
        }

    }


    void updateStateActions()
    {
        switch (currentState)
        {
            case PlayerStates.grounded:
                isGrounded = true;
                groundedLogic();
                //resetCameraPitch();
                break;
            case PlayerStates.airborne:
                isGrounded = false;
                

                

                //detect wallrunning

                if(Physics.Raycast(orientation.position,orientation.right,out wallHit,0.7f))
                {
                    TransitionToAirState(AirborneStates.wallRunningRight);
                }
                else if(Physics.Raycast(orientation.position, -orientation.right,out wallHit, 0.7f))
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

                //handles moving platforms
                
                if (currentMovingPlatform != null)
                {
                    // Calculate the target position by adding the platform's velocity
                    applyMovingPlatform();
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
                //resetCameraPitch();
                useNormalGravity();
                break;

            case AirborneStates.wallRunningLeft:
                // Add logic specific to wall running left
                if(initiateJump)
                {
                    jumpFromWall(-wallHit.normal.normalized);
                }
                //pitchCamera(-1);

                useWallrunningGravity();
                break;

            case AirborneStates.wallRunningRight:
                // Add logic specific to wall running right
                if (initiateJump)
                {
                    jumpFromWall(-wallHit.normal.normalized);
                }
                //pitchCamera(1);

                useWallrunningGravity();
                break;
        }
    }


    void dashingLogic()
    {
        if (Input.GetKeyDown(sprintKey) && !isDashing && canDash)
        {
            StartCoroutine(dash());
        }
        if(camHolder != null)
        {
            if (isDashing)
            {
                float targetFov = cameraFov + dashingFov;
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, timeToSprint * 10 * Time.deltaTime);
            }
            else
            {

                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, cameraFov, timeToSprint * 10 * Time.deltaTime);
            }
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
        camHolder.transform.position = headLocation.position;
    }

    void jumpFromWall(Vector3 wallDirection)
    {

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        initiateJump = false;
        rb.AddForce((transform.up * jumpForce) + (-wallDirection * wallJumpForce + (orientation.forward * wallRunForwardBoost)), ForceMode.Impulse);
        StartDashCooldown(0.8f);
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
        if(!isDashing)
        {
            rb.useGravity = true;
        }
        
    }
    void pitchCamera(float direction)
    {
        if (camHolder != null)
        {


            camScript.rotationZ = Mathf.Lerp(camScript.rotationZ, wallrunCameraDegrees * direction, cameraPitchSpeed * Time.deltaTime);
        }
    }

    void resetCameraPitch()
    {
        if (camHolder != null)
        {


            // Reset the camera rotation to its initial state
            camScript.rotationZ = Mathf.Lerp(camScript.rotationZ, 0, (wallrunCameraDegrees / cameraPitchSpeed) * Time.deltaTime);
        }
    }


    IEnumerator dash()
    {
        isDashing = true;
        rb.useGravity = false;
        velBeforeDash = rb.velocity;
        if(onSlope())
        {

            rb.AddForce(Vector3.ProjectOnPlane(moveDirection, slopeNormal.normal) * dashForce, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(moveDirection * dashForce, ForceMode.Impulse);
        }
        

        yield return new WaitForSeconds(dashDistance);

        rb.velocity = new Vector3(velBeforeDash.x,rb.velocity.y,velBeforeDash.z);
        rb.useGravity = true;
        isDashing = false;
        StartDashCooldown(dashCooldown);
    }
    IEnumerator cooldownDash(float time)
    {
        canDash = false;
        yield return new WaitForSeconds(time);
        canDash = true;
    }
    void StartDashCooldown(float time)
    {
        // Check if a coroutine is already running
        if (dashCooldownCoroutine != null)
        {
            // Stop the previous coroutine
            StopCoroutine(dashCooldownCoroutine);
        }

        // Start the new coroutine and store its reference
        dashCooldownCoroutine = StartCoroutine(cooldownDash(time));
    }

    void dragForces()
    {
        float forwardSpeed = Vector3.Dot(rb.velocity, orientation.forward);
        Vector3 forwardDragForce = -drag * forwardSpeed * orientation.forward;

        // Calculate drag force along the right direction
        float rightSpeed = Vector3.Dot(rb.velocity, orientation.right);
        Vector3 rightDragForce = -drag * rightSpeed * orientation.right;

        // Apply the drag forces
        rb.AddForce(forwardDragForce, ForceMode.Force);
        rb.AddForce(rightDragForce, ForceMode.Force);
    }



    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentMovingPlatform = collision.gameObject;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            
            
            currentMovingPlatform = null;
            ClearPreviousPlatformPosition(collision.gameObject);
        }
        

    }

    public void applyMovingPlatform()
    {
        Vector3 platformVelocity;
        RaycastHit hit;
        Vector3 targetPosition;
        if(Physics.Raycast(transform.position,-transform.up,out hit,2))
        {
            platformVelocity = hit.rigidbody.velocity;

            //change the physics metarial to high friction
            targetPosition = transform.position + platformVelocity;
            targetVel = targetPosition - transform.position;
        }
        else
        {
            platformVelocity = Vector3.zero;

            //low friction
            targetPosition = Vector3.zero;
            targetVel = Vector3.zero;
        }

        rb.velocity += targetVel * movingPlatformConstant;
        

        
    }
    public void ClearPreviousPlatformPosition(GameObject platform)
    {
        // Check if the platform exists in the dictionary
        if (previousPlatformPositions.ContainsKey(platform))
        {
            // If it does, remove it from the dictionary
            previousPlatformPositions.Remove(platform);
        }
        // Otherwise, do nothing
    }

}
