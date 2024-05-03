using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Cinemachine;

public enum PlayerState { Idle, Attack, QuickStep, Damage, Block, Dead}

public class PlayerMovement : Singleton<PlayerMovement>
{
    public static event Action PlayerDeath = null;

    public PlayerState state;

    [Header("Movement")]
    public CharacterController controller;
    public float speed = 10f;
    public float runSpeed = 10f;
    public float walkSpeed = 5f;
    public Transform orientation;
    public Vector3 inputDirection;
    public Vector3 moveDirection;
    public CinemachineFreeLook freeLook;

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
    public int health = 100;
    public int maxHealth = 100;
    public HealthBar healthBar;
    public bool cantDie;
    public Animator anim;
    public AudioSource audioSource;

    public List<GameObject> enemiesInRange;
    public GameObject targetEnemy;
    public float distance;
    public float nearestDistance = 1000f;

    public float x;
    public float z;
    public Transform cam;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;

    public bool inCutscene;
    bool onBoundary;

    public TMP_Text healthText; //temp
    public GameObject cantDieText;

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
        audioSource = GetComponent<AudioSource>();
        
        //UseRigidbody(false);
    }

    private void Update()
    {
        //Get the input from the player
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown("1"))
        {
            cantDie = !cantDie;
            cantDieText.SetActive(cantDie);
        }
        //ToggleCursorLockState();

        //if (Input.GetKeyDown("r"))
        //    SceneManager.LoadScene("Level");

        //if (Input.GetKeyDown(KeyCode.Escape))
        //    Application.Quit();

        if (state != PlayerState.Damage)
            _GM.CamUpdateFixed();

        //test change layer
        if (Input.GetKeyDown("8") && onBoundary)
            ChangeLayerTest();
        
    }

    private void FixedUpdate()
    {

        MovePlayer();
    }

    void MovePlayer()
    {
        //Checks if we are touching the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //Gravity
        if (gravityOn)
        {
            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            velocity.y += gravity * Time.deltaTime;

            if (controller.enabled == true)
                controller.Move(velocity * Time.deltaTime);
        }

        if (state == PlayerState.Dead || inCutscene)
            return;

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

        if (inputDirection.magnitude >= 0.1f && state == PlayerState.Idle)
        {
            if (inputDirection.magnitude > 0.6 && Input.GetAxisRaw("Walk") <= 0)
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
        if (Input.GetAxisRaw("Walk") <= 0)
            anim.SetFloat("movementSpeed", inputDirection.magnitude);
        else
        {
            if (inputDirection.magnitude >= 0.1f)
                anim.SetFloat("movementSpeed", 0.3f);
            else
                anim.SetFloat("movementSpeed", 0f);
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

    public void TakeDamage(int _damage)
    {
        if (state == PlayerState.Dead || cantDie)
            return;

        state = PlayerState.Damage;
        ActivateRB();

        anim.SetTrigger("Damage");
        health -= _damage;
        healthBar.UpdateHealthBar(health, maxHealth);
        Debug.Log(health);

        //if (healthText != null)
        //{
        //    healthText.text = "HP: " + health.ToString(); //temp
        //}

        if (health <= 0)
            Die();
    }

    public void Die()
    {
        state = PlayerState.Dead;
        freeLook.gameObject.SetActive(false);
        GetComponent<MannequinExplode>().Explode();
        anim.SetTrigger("Die");
        _AM.PlaySound(_AM.GetDeathSound(), audioSource);
        PlayerDeath();
    }

    public void RecoverHealth(int _recAmount)
    {
        health += _recAmount;
        if (health > maxHealth)
            health = maxHealth;
        healthBar.UpdateHealthBar(health, maxHealth);
        Debug.Log(health);

        if (healthText != null)
            healthText.text = "HP: " + health.ToString(); //temp
    }

    void UseRigidbody(bool _state)
    {
        rb.detectCollisions = _state;
        rb.isKinematic = !_state;
        rb.useGravity = _state;
        rbCollider.enabled = _state;
    }

    void ActivateRB()
    {
        rb.isKinematic = false; // Activates Rigidbody
        controller.enabled = false;
    }

    public void ActivateCC()
    {
        rb.isKinematic = true; // Deactivates Rigidbody
        controller.enabled = true;
    }

    public void ClearDeadEnemy(GameObject _enemy)
    {
        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            if (enemiesInRange[i] == _enemy)
            {
                enemiesInRange.Remove(_enemy);
                return;
                //break;
            }
        }
    }

    private void OnEnable()
    {
        Enemy.OnEmemyDie += ClearDeadEnemy;
    }

    private void OnDisable()
    {
        Enemy.OnEmemyDie -= ClearDeadEnemy;
    }

    void ChangeLayerTest()
    {
        int breakThroughLayer = LayerMask.NameToLayer("BreakThrough");
        gameObject.layer = breakThroughLayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary"))
            onBoundary = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boundary"))
            onBoundary = false;
    }
}
