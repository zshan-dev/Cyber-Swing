using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMovementGrappling : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed; // Unused variable
    public float walkSpeed;
    public float sprintSpeed;
    public float swingSpeed;
    public float wallrunSpeed;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Camera Effects")]
    public PlayerCam cam;
    public float grappleFov = 95f;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        freeze,
        grappling,
        swinging,
        walking,
        wallrunning,
        sprinting,
        crouching,
        air
    }

    public bool freeze;
    public bool wallrunning;
    public bool activeGrapple;
    public bool swinging;

    private void Start()
    {
        // Get the Rigidbody component and freeze rotation
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Set the jump flag to true initially
        readyToJump = true;

        // Store the initial scale of the player for crouch
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        // Check if the player is on the ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        // Handle player input
        MyInput();
        
        // Adjust speed based on player state
        SpeedControl();
        
        // Handle different movement states
        StateHandler();

        // Handle drag based on the ground and grapple state
        if (grounded && !activeGrapple)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        // Display debug text
        TextStuff();
    }

    private void FixedUpdate()
    {
        // Move the player based on input
        MovePlayer();
    }

    private void MyInput()
    {
        // Get horizontal and vertical input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Check for jump input
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump(); // Perform jump
            Invoke(nameof(ResetJump), jumpCooldown); // Reset jump cooldown
        }

        // Start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // Stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {

        //Mode - Wallrunning
        if(wallrunning){
            state = MovementState.wallrunning;
            moveSpeed = wallrunSpeed;
        }
        // Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }

        // Mode - Grappling
        else if (activeGrapple)
        {
            state = MovementState.grappling;
            moveSpeed = sprintSpeed;
        }

        // Mode - Swinging
        else if (swinging)
        {
            state = MovementState.swinging;
            moveSpeed = swingSpeed;
        }

        // Mode - Crouching
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        if (activeGrapple) return;
        if (swinging) return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (activeGrapple) return;

        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    // Method to handle player jumping
    private void Jump()
    {
        // Set the flag to indicate exiting a slope
        exitingSlope = true;

        // Reset the vertical (y) velocity to zero
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Apply an upward force to perform the jump using impulse
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // Method to reset jump-related flags
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    // Flag to enable movement on the next touch (collision)
    private bool enableMovementOnNextTouch;

    // Method to initiate a jump to a specified position with trajectory height
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        // Calculate the required jump velocity based on the target position and trajectory height
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);

        // Set the velocity after a short delay
        Invoke(nameof(SetVelocity), 0.1f);

        // Reset restrictions after 3 seconds
        Invoke(nameof(ResetRestrictions), 3f);
    }

    // Vector to store the calculated jump velocity
    private Vector3 velocityToSet;

    // Method to set the calculated jump velocity and adjust camera field of view
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;

        // Adjust camera field of view during grapple
        cam.DoFov(grappleFov);
    }

    // Method to reset grapple-related restrictions
    public void ResetRestrictions()
    {
        activeGrapple = false;
        
        // Reset camera field of view
        cam.DoFov(85f);
    }

    // Event handler when a collision occurs
    private void OnCollisionEnter(Collision collision)
    {
        // Check if movement should be enabled after the next touch
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;

            // Reset grapple-related restrictions
            ResetRestrictions();

            // Uncomment the line below to cancel active grapples using DualHooks component
            //GetComponent<DualHooks>().CancelActiveGrapples();
        }
    }

    // Method to check if the player is on a slope
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    // Method to get the movement direction on a slope
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    // Method to calculate the jump velocity required to reach a target position with a specified trajectory height
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    #region Text & Debugging

    // TextMeshPro UI elements for displaying speed and movement state
    public TextMeshProUGUI text_speed;
    public TextMeshProUGUI text_mode;

    // Method to update and display text for speed and movement state
    private void TextStuff()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Display speed information based on whether the player is on a slope or not
        if (OnSlope())
            text_speed.SetText("Speed: " + Round(rb.velocity.magnitude, 1) + " / " + Round(moveSpeed, 1));
        else
            text_speed.SetText("Speed: " + Round(flatVel.magnitude, 1) + " / " + Round(moveSpeed, 1));

        // Display the current movement state
        text_mode.SetText(state.ToString());
    }

    // Static method for rounding a float value to a specified number of digits
    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    #endregion
}
