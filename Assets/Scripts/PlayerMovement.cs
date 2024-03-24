using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { Idle, Attack, QuickStep, Damage}

public class PlayerMovement : Singleton<PlayerMovement>
{
    #region Rigidbody
    //[Header("Movement")]
    //public float moveSpeed = 6;
    //public float runSpeed = 6;
    //public float walkSpeed = 3;

    //public float groundDrag = 5;

    //[Header("GroundCheck")]
    //public float playerHeight;
    //public LayerMask ground;
    //bool grounded;

    //[Header("Slope Handling")]
    //public float maxSlopeAngle;
    //private RaycastHit slopeHit;
    //private bool exitingSlope;

    //[Header("Step Handling")]
    //public GameObject stepRayUpper;
    //public GameObject stepRayLower;
    //public float stepHeight = 0.3f;
    //public float stepSmooth = 2f;

    //public Transform orientation;

    //float horizontalInput;
    //float verticalInput;

    //Vector3 moveDirection;

    //Rigidbody rb;
    //Animator anim;

    //private void Start()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    rb.freezeRotation = true;
    //    anim = GetComponent<Animator>();

    //    stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    //}

    //private void Update()
    //{
    //    //ground check
    //    grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

    //    PlayerInput();
    //    SpeedControl();

    //    //handle drag
    //    if (grounded)
    //        rb.drag = groundDrag;
    //    else
    //        rb.drag = 0;

    //    //Debug.Log(moveDirection.magnitude);

    //}

    //private void FixedUpdate()
    //{
    //    MovePlayer();
    //    //StepClimb();
    //}

    //private void PlayerInput()
    //{
    //    horizontalInput = Input.GetAxisRaw("Horizontal");
    //    verticalInput = Input.GetAxisRaw("Vertical");

    //}

    //private void MovePlayer()
    //{
    //    //calculate move direction
    //    moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

    //    //Handles animation
    //    anim.SetFloat("movementSpeed", moveDirection.magnitude);

    //    //on slope
    //    if(OnSlope())
    //    {
    //        rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

    //        if (rb.velocity.y > 0)
    //            rb.AddForce(Vector3.down * 80f, ForceMode.Force);
    //    }

    //    //on ground
    //    else if (grounded)
    //        rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);

    //    //turn gravity off while on slope
    //    rb.useGravity = !OnSlope();
    //}

    //private void SpeedControl()
    //{

    //    //limiting speed on slope
    //    if(OnSlope() && !exitingSlope)
    //    {
    //        if (rb.velocity.magnitude > moveSpeed)
    //            rb.velocity = rb.velocity.normalized * moveSpeed;
    //    }

    //    //limiting speed on ground/air
    //    else
    //    {
    //        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

    //        //limit velocity if needed
    //        if (flatVel.magnitude > moveSpeed)
    //        {
    //            Vector3 limitedVel = flatVel.normalized * moveSpeed;
    //            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
    //        }
    //    }
    //}

    //private bool OnSlope()
    //{
    //    if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
    //    {
    //        float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
    //        return angle < maxSlopeAngle && angle != 0;
    //    }

    //    return false;
    //}

    //private Vector3 GetSlopeMoveDirection()
    //{
    //    return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    //}

    //void StepClimb()
    //{
    //    RaycastHit hitLower;
    //    if(Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
    //    {
    //        RaycastHit hitUpper;
    //        if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
    //            rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
    //    }

    //    RaycastHit hitLower45;
    //    if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitLower45, 0.1f))
    //    {
    //        RaycastHit hitUpper45;
    //        if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f))
    //            rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
    //    }

    //    RaycastHit hitLowerMinus45;
    //    if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, 0.1f))
    //    {
    //        RaycastHit hitUpperMinus45;
    //        if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f))
    //            rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
    //    }
    //}
    #endregion

    public PlayerState state;

    [Header("Movement")]
    public CharacterController controller;
    public float speed = 10f;
    public float runSpeed = 10f;
    public float walkSpeed = 5f;
    public Transform orientation;
    public Vector3 inputDirection;
    public Vector3 moveDirection;

    [Header("Gravity")]
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Vector3 velocity;
    public bool isGrounded;
    public bool gravityOn = true;

    [Header("Slopes")]
    public float slopeForce;
    public float slopeForceRayLength;

    [Header("Misc")]
    public float health = 100;
    public float maxHealth = 100;
    public Animator anim;

    public List<GameObject> enemiesInRange;
    public GameObject targetEnemy;
    public float distance;
    public float nearestDistance = 1000f;

    float x;
    float z;
    public Transform cam;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;

    //Rb Stuff
    public Rigidbody rb;
    public Collider rbCollider; //is supposed to contain Capsual Collider, NOT Character Controller

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        speed = runSpeed;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rbCollider = GetComponent<Collider>();
        
        //UseRigidbody(false);

        ToggleCursorLockState();
    }

    private void Update()
    {
        //Get the input from the player
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown("1"))
            ToggleCursorLockState();

        
    }

    private void FixedUpdate()
    {

        MovePlayer();
    }

    void MovePlayer()
    {
        //Checks if we are touching the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //Move the player
        #region Method A
        //inputDirection = orientation.forward * z + orientation.right * x;

        //if (inputDirection.magnitude >= 0.1f)
        //{
        //    if (inputDirection.magnitude > 0.6)
        //        speed = runSpeed;
        //    else
        //        speed = walkSpeed;

        //    moveDirection = orientation.forward * z + orientation.right * x;

        //    if (state == PlayerState.Idle)
        //        controller.Move(moveDirection * speed * Time.deltaTime);
        //}
        #endregion

        #region Method B
        inputDirection = new Vector3(x, 0f, z);

        if (inputDirection.magnitude >= 0.1f)
        {
            if (inputDirection.magnitude > 0.6)
                speed = runSpeed;
            else
                speed = walkSpeed;

            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(x, targetAngle, z) * Vector3.forward;

            if (state == PlayerState.Idle)
                controller.Move(moveDirection * speed * Time.deltaTime);
        }
        #endregion

        //Slope Movement
        if (moveDirection.magnitude != 0 && OnSlope())
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);

        //Handles animation
        anim.SetFloat("movementSpeed", inputDirection.magnitude);

        //Gravity
        if (gravityOn)
        {
            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    private bool OnSlope()
    {
        if (!isGrounded)
            return false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength))
            if (hit.normal != Vector3.up)
                return true;
        return false;
    }

    public void RecieveDamage(float _damage)
    {
        state = PlayerState.Damage;
        anim.SetTrigger("Damage");
        health -= _damage;
        Debug.Log(health);
    }

    public void RecoverHealth(float _recAmount)
    {
        health += _recAmount;
        if (health > maxHealth)
            health = maxHealth;
        Debug.Log(health);
    }

    void UseRigidbody(bool _state)
    {
        rb.detectCollisions = _state;
        rb.isKinematic = !_state;
        rb.useGravity = _state;
        rbCollider.enabled = _state;
    }

    void ToggleCursorLockState()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.None;
        else if (Cursor.lockState == CursorLockMode.None)
            Cursor.lockState = CursorLockMode.Locked;
    }
}
