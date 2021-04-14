using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;

    readonly private KeyCode grappleKey = KeyCode.Mouse0;
    public float maxDistance = 100f;

    public LayerMask Grappleable;
    public Transform gunTip, playerCamera;
    public CharacterController controller;

    private int moveSpeed = 100;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(grappleKey)) StartGrapple();
        else if (Input.GetKeyUp(grappleKey)) StopGrapple();
    }

    private void LateUpdate()
    {
        DrawRope();
        MoveTowardsTarget();
    }

    void DrawRope()
    {
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }

    RaycastHit hit;
    void StartGrapple()
    {
        if(Physics.Raycast(origin: playerCamera.position, direction: playerCamera.forward, out hit, maxDistance))
        {
            grapplePoint = hit.point;
            float distanceFromPoint = Vector3.Distance(playerCamera.position, grapplePoint);

        }
    }

    void MoveTowardsTarget()
    {
        var change = 100 * Time.deltaTime;
        Vector3 movement = Vector3.MoveTowards(playerCamera.position, grapplePoint, change);
        controller.Move(movement);
    }

    void StopGrapple()
    {
    }
}
