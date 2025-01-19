using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingDone : MonoBehaviour
{
    // References
    [Header("References")]
    private PlayerMovementGrappling pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    // Grappling parameters
    [Header("Grappling")]
    public float maxGrappleDistance = 25f;
    public float grappleDelayTime = 0.5f;
    public float overshootYAxis = 2f;

    private Vector3 grapplePoint;

    // Cooldown parameters
    [Header("Cooldown")]
    public float grapplingCd = 2.5f;
    private float grapplingCdTimer;

    // Input settings
    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    // Initialization
    private void Start()
    {
        pm = GetComponent<PlayerMovementGrappling>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Input handling
        if (Input.GetKeyDown(grappleKey))
            StartGrapple();

        // Cooldown timer
        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    // LateUpdate is called after all Update functions have been called
    private void LateUpdate()
    {
        // Update line renderer position when grappling
        if (grappling)
            lr.SetPosition(0, gunTip.position);
    }

    // Start the grapple process
    public void StartGrapple()
    {
        // Check if the grapple is on cooldown
        if (grapplingCdTimer > 0)
            return;

        grappling = true;

        pm.freeze = true;

        RaycastHit hit;
        // Cast a ray from the camera to find a grapple point
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            // Execute grapple after a delay
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            // If no grapple point is found, create a point in the direction of the camera
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            // Stop grapple after a delay
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        // Enable line renderer and set its end position to the grapple point
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    // Execute the grapple movement
    public void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOfArc = grapplePointRelativeYPos + overshootYAxis;

        // Adjust the arc if the grapple point is below the player
        if (grapplePointRelativeYPos < 0)
            highestPointOfArc = overshootYAxis;

        // Jump to the grapple point with the specified arc
        pm.JumpToPosition(grapplePoint, highestPointOfArc);

        // Stop grapple after a delay
        Invoke(nameof(StopGrapple), 1f);
    }

    // Stop the grapple process
    public void StopGrapple()
    {
        pm.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }

    // Callback when the object is touched
    public void OnObjectTouch()
    {
        // Unused code (commented out)
        // if (pm.activeGrapple) StopGrapple();
    }

    // Check if the player is currently grappling
    public bool IsGrappling()
    {
        return grappling;
    }

    // Get the current grapple point
    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    /*
    // Unused code (commented out)
    void some()
    {
        // Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            rb.velocity = Vector3.zero;
            moveSpeed = 0f;
        }
        else if (activeGrapple)
        {
            state = MovementState.grappling;
            moveSpeed = sprintSpeed;
        }
    }

    // Unused code (commented out)
    private bool enableMovementOnNextTouch;
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        //activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
        Invoke(nameof(ResetRestrictions), 3f);
        enableMovementOnNextTouch = true;
    }
    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        rb.velocity = velocityToSet;
        print("velocityToSet: " + velocityToSet + " velocity: " + rb.velocity);
    }

    // Unused code (commented out)
    public void ResetRestrictions()
    {
        //activeGrapple = false;
    }

    // Unused code (commented out)
    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();
            GetComponent<Grappling_MLab>().StopGrapple();
        }
    }
    */
}
