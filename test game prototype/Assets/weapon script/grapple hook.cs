using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class grapplehook : Weapon
{
    [Space]

    [Header("grapple hook settings")]

    public LayerMask grappleMask;

    Vector3 grapplePoint;

    GameObject player;

    bool isGrappling;

    public Transform grappleShootPoint;
    public Transform targetPointVisual;
    public float sphereCastRadius;
    LineRenderer grappleRender;
    SpringJoint joint;

    [Range(0f, 1)]
    public float rendererWidth;


    [Space]
    public float grapplePointLaunchInfluence;
    public float grappleLaunchForce;
    public float range;
    public float pullInStrength;
    public float minPullStrength;
    public float pullInDamp;
    public float thresholdDistance = 1f;
    public float thresholdDistanceMultiplier;
    public float maxYVel;
    [Space]
    public float forwardBoost;
    public float lookMinToBoost;


    Vector3 playerGrappleStartPos;
    float distanceFromGrapplePoint;
    float pullInMultiplier;

    Vector3 playerVelocity;

    Rigidbody playerRb;

    RaycastHit predictionPoint;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody>();
    }


    protected override void Update()
    {
        base.Update();
        playerVelocity = playerRb.velocity;
        distanceFromGrapplePoint = Vector3.Distance(player.transform.position, grapplePoint);


        if (isGrappling)
        {
            float pullInSpeed = Mathf.Clamp(pullInStrength * pullInMultiplier, minPullStrength, pullInStrength);
            Debug.Log(pullInSpeed);
            joint.maxDistance -= pullInSpeed * Time.deltaTime;
            

            

            //stop grapple conditions
            if (Input.GetMouseButtonDown(1))
            {
                stopGrapple();
                launchPlayerToGrapplePoint();
            }


            if (Vector3.Distance(grapplePoint, player.transform.position) <= 2 || Input.GetMouseButtonUp(0))
            {
                stopGrapple();
            }






            


        }


        if(!isGrappling)
        {
            if (targetPointVisual != null)
            {
                RaycastHit rayHit;
                RaycastHit sphereCastHit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out rayHit, range, grappleMask))
                {
                    
                    targetPointVisual.gameObject.SetActive(true);
                    targetPointVisual.position = rayHit.point;
                    predictionPoint = rayHit;
                }
                else if (Physics.SphereCast(Camera.main.transform.position, sphereCastRadius, Camera.main.transform.forward, out sphereCastHit, range, grappleMask))
                {
                    
                    targetPointVisual.gameObject.SetActive(true);
                    targetPointVisual.position = sphereCastHit.point;
                    predictionPoint = sphereCastHit;
                }
                else
                {
                    
                    targetPointVisual.gameObject.SetActive(false);
                    predictionPoint.point = Vector3.zero;
                }

            }
        }
        else
        {
            targetPointVisual.gameObject.SetActive(false);
            predictionPoint.point = Vector3.zero;
        }



    }


    

    private void FixedUpdate()
    {
        if (isGrappling)
        {
            // Calculate the direction from the player to the grappling point
            Vector3 directionToGrapplingPoint = grapplePoint - transform.position;
            directionToGrapplingPoint.Normalize();

            // Calculate the dot product between the camera's forward vector and the direction to the grappling point
            float dotProduct = Vector3.Dot(Camera.main.transform.forward, directionToGrapplingPoint);

            // Check if the dot product is less than a certain threshold (indicating the camera is not facing directly towards the grappling point)
            
            float clampedDotProduct = Mathf.Clamp (dotProduct, 0.0f, 1.0f);
            pullInMultiplier = clampedDotProduct;
            float multiplier = (1 - clampedDotProduct) * forwardBoost; //reverse multiplier here
            Vector3 force = Camera.main.transform.forward * multiplier;
            playerRb.AddForce(force, ForceMode.Force);
            //clamp max y vel
            if(playerRb.velocity.y > maxYVel)
            {
                playerRb.velocity = new Vector3(playerRb.velocity.x, maxYVel, playerRb.velocity.z);
            }

        }
    }

    private void LateUpdate()
    {
        if (isGrappling)
        {
            //update renderer positions;
            Vector3[] lineRendererPositions = { grappleShootPoint.position, grapplePoint };

            grappleRender.SetPositions(lineRendererPositions);
        }
    }

    protected override void reload()
    {
        // no reloading needed
    }

    protected override void DefineProjectile()
    {
        //no projectile
    }



    protected override void Shoot()
    {
        if(!isGrappling)
        {
            startGrapple();
        }
        else
        {
            stopGrapple();
        }
    }

    public bool IsPlayerOnOtherSide()
    {
        if (isGrappling)
        {
            // Calculate the vector from the grapple point to the player's current position (ignoring y-axis)
            Vector3 playerToGrapplePoint = new Vector3(player.transform.position.x - grapplePoint.x, 0f, player.transform.position.z - grapplePoint.z);

            // Calculate the vector from the grapple point to the player's initial position (ignoring y-axis)
            Vector3 playerToStartPos = new Vector3(playerGrappleStartPos.x - grapplePoint.x, 0f, playerGrappleStartPos.z - grapplePoint.z);

            // Normalize the vectors
            playerToGrapplePoint.Normalize();
            playerToStartPos.Normalize();

            // Calculate the dot product between the two vectors
            float dotProduct = Vector3.Dot(playerToGrapplePoint, playerToStartPos);

            // Calculate the angle between the vectors (in radians)
            float angle = Mathf.Acos(dotProduct);

            // Convert the angle from radians to degrees
            angle = angle * Mathf.Rad2Deg;

            // Check if the angle is greater than a threshold value (e.g., 90 degrees)
            if (angle > 90f)
            {
                if (distanceFromGrapplePoint < thresholdDistance)
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
        return false;
    }





    void startGrapple()
    {
        

        if(predictionPoint.point != Vector3.zero)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
            isGrappling = true;
            //set grapple joint
            
            grapplePoint = predictionPoint.point;
            playerGrappleStartPos = player.transform.position;
            joint = player.AddComponent<SpringJoint>();
            Debug.Log(joint);

            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromGrapple = Vector3.Distance(grapplePoint, player.transform.position);
            thresholdDistance = distanceFromGrapple / thresholdDistanceMultiplier;

            joint.maxDistance = 1f * distanceFromGrapple;
            joint.minDistance = 0f * distanceFromGrapple;

            joint.spring = 4.5f;
            joint.damper = pullInDamp;
            joint.massScale = 4.5f;

            //set renderer
            
            grappleRender = grappleShootPoint.gameObject.AddComponent<LineRenderer>(); ;
            grappleRender.startWidth = rendererWidth;
            grappleRender.endWidth = rendererWidth;
            
        }
            


    }

    void launchPlayerToGrapplePoint()
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        Vector3 grapplePointPos = grapplePoint;

        // Calculate the direction from the player to the grapple point
        Vector3 directionToGrapplePoint = grapplePointPos - player.transform.position;

        // Calculate the player's forward direction
        Vector3 playerForward = player.transform.forward + player.transform.up;

        // Calculate the combined direction by adding influence from the grapple point
        Vector3 combinedDirection = playerForward + directionToGrapplePoint.normalized * grapplePointLaunchInfluence;

        // Apply force to launch the player in the combined direction
        playerRb.AddForce(combinedDirection.normalized * grappleLaunchForce, ForceMode.Impulse);
    }




    void stopGrapple()
    {
        
        isGrappling = false;
        if(player != null)
        {
            if(player.GetComponent<SpringJoint>() != null)
            {
                Destroy(player.GetComponent<SpringJoint>());
            }
            
        }
        if(grappleRender!= null)
        {
            Destroy(grappleRender);
        }
       
            
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
        {
            Debug.Log("application playing");
            stopGrapple();
        }

    }

}
