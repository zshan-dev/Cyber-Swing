using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class implements wall-running mechanics for a player character.
public class WallRun : MonoBehaviour
{
    // Wallrunning parameters
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    private float wallRunTimer;

    // Input configuration
    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;
    private float horizontalInput;
    private float verticalInput;

    // Wall detection parameters
    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    // Exiting wall parameters
    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    // Gravity control
    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    // References
    [Header("References")]
    public Transform orientation;
    public PlayerCam cam;
    private PlayerMovementGrappling pm;
    private Rigidbody rb;

    private void Start()
    {
        // Initialize references
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovementGrappling>();
    }

    private void Update()
    {
        // Check for wall presence and manage wall-run states
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        // Perform wall-running movement when in wall-running state
        if (pm.wallrunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        // Perform raycasts to detect walls on the left and right sides.
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        // Check if the player is above a certain distance from the ground.
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        // Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallrunning)
                StartWallRun();

            // wallrun timer
            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            // wall jump
            if (Input.GetKeyDown(jumpKey))
                WallJump();
        }

        // State 2 - Exiting
        else if (exitingWall)
        {
            if (pm.wallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }

        // State 3 - None
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        // Initialize wall-running state
        pm.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        // Reset vertical velocity for smoother transitions
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Apply camera effects
        cam.DoFov(90f);
        if (wallLeft)
            cam.DoTilt(-5f);
        if (wallRight)
            cam.DoTilt(5f);
    }

    private void WallRunningMovement()
    {
        // Toggle gravity based on configuration
        rb.useGravity = useGravity;

        // Determine the wall normal based on the detected wall
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        // Calculate a vector pointing forward along the wall
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        // Choose the correct forward vector based on orientation
        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        // Apply forward force for wall running
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // Apply upwards/downwards force for wall climbing/descending
        if (upwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        if (downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);

        // Apply a force to push the player towards the wall
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rb.AddForce(-wallNormal * 100, ForceMode.Force);

        // Weaken gravity to simulate wall running
        if (useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
    }

    private void StopWallRun()
    {
        // Reset wall-running state and camera effects
        pm.wallrunning = false;
        cam.DoFov(80f);
        cam.DoTilt(0f);
    }

    private void WallJump()
    {
        // Trigger wall jump by entering exiting wall state
        exitingWall = true;
        exitWallTimer = exitWallTime;

        // Determine wall normal for jump direction
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        // Calculate the force to apply for the wall jump
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // Reset vertical velocity and apply the jump force impulsively
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
