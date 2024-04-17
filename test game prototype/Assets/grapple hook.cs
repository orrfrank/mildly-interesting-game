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
            if(Input.GetMouseButton(1))
            {
                joint.maxDistance -= pullInStrength * Time.deltaTime;
                joint.damper = 0;
            }
            else
            {
                joint.damper = Mathf.Lerp(joint.damper,pullInDamp,(pullInDamp / 2) * Time.deltaTime);
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
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, grappleMask) && range > Vector3.Distance(grapplePoint, player.transform.position))
        {
            Debug.Log(hit.transform);
            isGrappling = true;
            //set grapple joint
            grapplePoint = hit.point;
            joint = player.AddComponent<SpringJoint>();

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

        

        
    }



    void stopGrapple()
    {
        isGrappling = false;
        Destroy(player.GetComponent<SpringJoint>());
        Destroy(grappleRender);
    }


}
