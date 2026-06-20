using AudioSystem;
using UnityEngine;
using UnityEngine.InputSystem; // Required for InputAction.CallbackContext

public class PlayerCharacterController : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed = 7f;
    public float groundDrag = 4f;
    public float airMultiplier = 0.5f;

    [Header("Jumping")]
    public float jumpForce = 12f;
    public float jumpCooldown = 0.2f;
    private bool readyToJump;
    private bool jumpPressed;

    [Header("Dashing")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 2f;
    public bool readyToDash = true;
    private bool isDashing;
    private Vector3 lockedDashDirection;
    private float dashCooldownTimer;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    public float slopeDownForce = 30f;

    [Header("References")]
    public Transform orientation;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;

    public MovementState state;

    public enum MovementState
    {
        Walking,
        Air,
    }

    private EventBinding<PauseGameStateChangedEvent> pauseGameStateChangedBinding;

    private void OnEnable()
    {
        pauseGameStateChangedBinding = new EventBinding<PauseGameStateChangedEvent>(OnPauseGameStateChanged);
        EventBus<PauseGameStateChangedEvent>.Register(pauseGameStateChangedBinding);
    }

    private void OnDisable()
    {
        EventBus<PauseGameStateChangedEvent>.Deregister(pauseGameStateChangedBinding);
    }

    private void OnPauseGameStateChanged(PauseGameStateChangedEvent e)
    {
        //
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        readyToDash = true;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;
        }
        else if (context.canceled)
        {
            jumpPressed = false;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && readyToDash && !isDashing)
        {
            Dash();
        }
    }

    private void Update()
    {
        // Ground check
        float currentHeight = transform.localScale.y * playerHeight;
        grounded = Physics.Raycast(transform.position, Vector3.down, currentHeight * 0.5f + 0.1f, whatIsGround);

        JumpCheck();
        DashCooldownCheck();
        StateHandler();
        SpeedControl();

        // Drag handling
        if (grounded && !isDashing)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void JumpCheck()
    {
        if (jumpPressed && readyToJump && grounded && !isDashing)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void Dash()
    {
        readyToDash = false;
        isDashing = true;

        lockedDashDirection = new Vector3(orientation.forward.x, 0f, orientation.forward.z).normalized;

        rb.useGravity = false;

        dashCooldownTimer = Time.time + dashCooldown;

        SoundManager.instance.CreateSound().WithSoundData("Dash").WithrandomPitch().Play();

        Invoke(nameof(StopDash), dashDuration);
    }

    private void StopDash()
    {
        isDashing = false;
        rb.useGravity = true;

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > walkSpeed)
        {
            rb.linearVelocity = flatVel.normalized * walkSpeed + new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    private void DashCooldownCheck()
    {
        if (!readyToDash && Time.time >= dashCooldownTimer && grounded)
        { 
            readyToDash = true;
        }
    }    

    private void StateHandler()
    {
        // Mode - Walking
        if (grounded)
        {
            state = MovementState.Walking;
            moveSpeed = walkSpeed;
        }
        // Mode - Air
        else
        {
            state = MovementState.Air;
            moveSpeed = walkSpeed;
        }
    }

    private void MovePlayer()
    {
        if(isDashing)
        {
            rb.linearVelocity = lockedDashDirection * dashSpeed;
            return;
        }

        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        // On Slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        // On Ground
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        // In Air
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        if (OnSlope() && !exitingSlope)
        {
            rb.useGravity = false;
            rb.AddForce(Vector3.down * slopeDownForce, ForceMode.Force);
        }
        else
        {
            rb.useGravity = true;
        }
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed && !isDashing)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}