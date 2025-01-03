using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Cinemachine;

public enum PlayerState { Idle, Attack, QuickStep, Damage, Block, Dead}

public class PlayerMovement : Singleton<PlayerMovement>, IDamageable
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
    public float targetAngle;
    public Vector3 moveDirection;
    public bool lockRotation;
    public CinemachineFreeLook freeLook;

    [Header("Gravity")]
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private Vector3 velocity;
    public bool isGrounded;
    public bool gravityOn = true;

    [Header("Misc")]
    public int health = 100;
    public int maxHealth = 100;
    public HealthBar healthBar;
    public bool canCancel = true;
    public float heavyTimer;
    public bool cantDie;
    public Animator anim;
    public AudioSource audioSource;
    PlayerAttack attack;
    public AttackProperties attackProperties;

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
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        speed = runSpeed;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rbCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        attack = GetComponent<PlayerAttack>();
        attackProperties = GetComponent<AttackProperties>();
        
        //UseRigidbody(false);
    }

    private void Update()
    {
        //Get the input from the player
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        //if (Input.GetKeyDown("1"))
        //{
        //    cantDie = !cantDie;
        //    cantDieText.SetActive(cantDie);
        //}
        //ToggleCursorLockState();

        //if (Input.GetKeyDown("r"))
        //    SceneManager.LoadScene("Level");

        //if (Input.GetKeyDown(KeyCode.Escape))
        //    Application.Quit();

        if (state != PlayerState.Damage)
            _GM.CamUpdateFixed();

        //test change layer
        //if (Input.GetKeyDown("8") && onBoundary)
        //    ChangeLayerTest();

        //Stop game for screenshot
        //if (Input.GetButtonDown("ScreenPause"))
        //{
        //    if (Time.timeScale == 1)
        //        Time.timeScale = 0;
        //    else
        //        Time.timeScale = 1;
        //}

        if (heavyTimer > 0)
        {
            heavyTimer -= Time.deltaTime;
        }

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

        //Movement Code
        inputDirection = new Vector3(x, 0f, z);

        if (inputDirection.magnitude >= 0.1f)
        {
            //player speed
            if (inputDirection.magnitude > 0.6 && Input.GetAxisRaw("Walk") <= 0)
                speed = runSpeed;
            else
                speed = walkSpeed;

            //Get direction of movement
            targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //Set player's angle to direction of movement
            if (state == PlayerState.Idle || state == PlayerState.Attack && lockRotation == false)
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);


            moveDirection = Quaternion.Euler(x, targetAngle, z) * Vector3.forward;

            //Moves Player
            if (state == PlayerState.Idle)
                controller.Move(moveDirection * speed * Time.deltaTime);
        }

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

    public void TakeDamage(int _damage, int _damageAnim = 1)
    {
        if (state == PlayerState.Dead || cantDie)
            return;

        state = PlayerState.Damage;
        ActivateRB();

        anim.SetTrigger("Damage" + _damageAnim);
        health -= _damage;
        _GM.damageRecieved += _damage;
        healthBar.UpdateHealthBar(health, maxHealth);
        Debug.Log(health);
        attack.ExitBlock();

        if (health <= 0)
            Die();
    }

    public void Damage(int _damage, int _anim = 1)
    {
        TakeDamage(_damage, _anim);
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

    public void ActivateCancel()
    {
        canCancel = true;
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
