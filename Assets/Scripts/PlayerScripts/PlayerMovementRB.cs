using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementRB : MonoBehaviour
{
    [Header("Movement")]
    private float speed;
    public float walkSpeed = 7f;
    public float runSpeed = 10f;

    public float groundDrag = 5f;

    [Header("Jumping")]
    public float jumpForce = 12f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.4f;
    bool readyToJump;

    [Header("Floating Capsual")]
    public float rideHeight = 0.5f;
    public bool doneFalling;
    public float groundSpringStrength = 1000;
    public float groundSpringDamper = 60;
    public float airSpringStrength = 10;
    public float airSpringDamper = 30;
    public float rideSpringStrength = 1000;
    public float rideSpringDamper = 60;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    public Transform groundCheckPoint;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    public float moveDirMagnitude;

    Rigidbody rb;

    public LayerMask layerMask;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    [Header("Other")]
    public Animator anim;
    public float targetAngle;
    public Transform cam;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // ground check
        //grounded = grounded = Physics.Raycast(transform.position, Vector3.down, (playerHeight + 1.5f) * 0.5f, whatIsGround);
        grounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, (playerHeight + 1.5f) * 0.5f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (grounded)
        {
            rb.drag = groundDrag;

        }
        else
        {
            rb.drag = 0;

        }
    }

    private void FixedUpdate()
    {
        MovePlayer();

        FloatingRigidbody();
        
    }

    private void FloatingRigidbody()
    {
        Debug.DrawLine(groundCheckPoint.position, new Vector3(groundCheckPoint.position.x, groundCheckPoint.position.y - rideHeight, groundCheckPoint.position.z), Color.yellow);

        RaycastHit rayHit;
        if (Physics.Raycast(groundCheckPoint.position, groundCheckPoint.TransformDirection(Vector3.down), out rayHit, rideHeight, layerMask))
        {
            if (!doneFalling)
            {
                //rb.useGravity = false;
                //Debug.DrawLine(transform.position, transform.TransformDirection(Vector3.down) * rayHit.distance, Color.yellow);
                Debug.Log("Hit");


                Vector3 vel = rb.velocity;
                Vector3 rayDir = transform.TransformDirection(Vector3.down);

                float rayDirVel = Vector3.Dot(rayDir, vel);

                float relVel = rayDirVel;

                float x = rayHit.distance - rideHeight;

                float springForce = (x * rideSpringStrength) - (relVel * rideSpringDamper);

                rb.AddForce(rayDir * springForce);

                Debug.DrawLine(transform.position, transform.position + (rayDir * springForce), Color.yellow);

                rb.useGravity = false;

                rideSpringStrength = groundSpringStrength;
                rideSpringDamper = groundSpringDamper;

            }

            //rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        else
        {
            rb.useGravity = true;

            rideSpringStrength = airSpringStrength;
            rideSpringDamper = airSpringDamper;

        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler()
    {
        // Mode - Walking
        if (grounded)
        {
            state = MovementState.walking;
            //speed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirMagnitude = moveDirection.magnitude;

        //move speed
        if (moveDirection.magnitude > 0.6 && Input.GetAxisRaw("Walk") <= 0)
            speed = runSpeed;
        else if (moveDirection.magnitude >= 0.1)
            speed = walkSpeed;
        else
            speed = 0;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * speed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * speed * 10f * airMultiplier, ForceMode.Force);


        //Handles animation
        if (Input.GetAxisRaw("Walk") <= 0)
            anim.SetFloat("movementSpeed", moveDirection.magnitude);
        else
        {
            if (moveDirection.magnitude >= 0.1f)
                anim.SetFloat("movementSpeed", 0.3f);
            else
                anim.SetFloat("movementSpeed", 0f);
        }
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > speed)
                rb.velocity = rb.velocity.normalized * speed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > speed)
            {
                Vector3 limitedVel = flatVel.normalized * speed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, (playerHeight + 1.5f) * 0.5f + 0.3f))
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
