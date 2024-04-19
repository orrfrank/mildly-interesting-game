using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class grapplehook : Weapon
{
    [Space]

    [Header("grapple hook settings")]

    public LayerMask grappleMask;

    Vector3 grapplePoint;

    GameObject player;

    bool isGrappling;

    public Transform grappleShootPoint;

    LineRenderer grappleRender;
    SpringJoint joint;

    [Range(0f, 1)]
    public float rendererWidth;


    [Space]
    public float grapplePointLaunchInfluence;
    public float grappleLaunchForce;
    public float range;
    public float pullInStrength;
    public float pullInDamp;
    

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    protected override void Update()
    {
        base.Update();

        if (isGrappling)
        {
            if(Input.GetMouseButtonDown(1))
            {
                stopGrapple();
                launchPlayerToGrapplePoint();
            }


            if (Vector3.Distance(grapplePoint, player.transform.position) <= 2 || Input.GetMouseButtonUp(0))
            {
                stopGrapple();
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


    void startGrapple()
    {
        
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, grappleMask))
        {
            
            isGrappling = true;
            //set grapple joint
            
            grapplePoint = hit.point;
            
            joint = player.AddComponent<SpringJoint>();
            Debug.Log(joint);

            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromGrapple = Vector3.Distance(grapplePoint, player.transform.position);

            joint.maxDistance = 0.8f * distanceFromGrapple;
            joint.minDistance = 0f * distanceFromGrapple;

            joint.spring = 4.5f;
            joint.damper = pullInDamp;
            joint.massScale = 4.5f;

            //set renderer
            
            grappleRender = grappleShootPoint.gameObject.AddComponent<LineRenderer>(); ;
            grappleRender.startWidth = rendererWidth;
            grappleRender.endWidth = rendererWidth;
            
        }
        Debug.Log(hit.transform);
            


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
        Destroy(player.GetComponent<SpringJoint>());
        Destroy(grappleRender);
            
    }
    


}
