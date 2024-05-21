using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Roam, Chase, Attack, Damage, Die, PlayerDead}
public class Enemy : GameBehaviour
{
    public static event Action<GameObject> OnEmemyDie = null;

    [Header("General")]
    public EnemyState state;
    public int health = 100;
    public int maxHealth = 100;
    public int lowPlayerHealth = 40;
    public Animator anim;
    public NavMeshAgent agent;
    public Transform groundCheck;
    public bool changeState;
    public Rigidbody rb;
    public Vector3 moveDir;
    float distToPlayer;
    public bool attacking;
    public Transform orientation;
    public AudioSource audioSource;
    public AudioClip attackLandSFX;
    HealthBar healthBar;
    public GameObject healthPickUp;
    AttackProperties attackProperties;

    public float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;
    public bool isGrounded;

    public Vector3 position1;
    public Vector3 position2;
    public float enemyAngle;

    public bool onBoundary;
    
    
    [Header("Roam State")]
    public float walkSpeed = 1.5f;
    public bool walkpointSet;
    public bool inRange;
    Vector3 destPoint;
    [SerializeField] float moveRangeX = 5;
    [SerializeField] float moveRangeZ = 5;
    [SerializeField] float maxDistFromPlayer = 8;
    [SerializeField] float minDistFromPlayer = 1;
    [SerializeField] float maxDistFromDest = 1;

    [Header("Chase State")]
    public float runSpeed = 6f;
    public float attackDistance = 1f;

    [Header("Attack State")]
    public float knockbackXZ;
    public float knockbackY;
    public int damage;
    public bool isHeavy;
    public int damageAnim;
    public bool attackBlocked;
    Vector3 lastPlayerLocation;

    [Header("Debug")]
    public bool canAttack = true;
    public bool canDie = true;


    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        healthBar = GetComponentInChildren<HealthBar>();
        attackProperties = GetComponent<AttackProperties>();
        StartCoroutine(PositionCheck());
    }

    
    void Update()
    {
        //Checks if we are touching the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (state == EnemyState.PlayerDead)
            return;

        healthBar.gameObject.transform.LookAt(_PLAYER.cam);

        moveDir = agent.desiredVelocity.normalized;

        moveDir = new Vector3(transform.right.magnitude * agent.desiredVelocity.x, 0f, transform.forward.magnitude * agent.desiredVelocity.z);

        //anim.SetFloat("moveX", moveDir.x - _PLAYER.transform.position.x);
        anim.SetFloat("moveY", transform.forward.magnitude * moveDir.z);

        distToPlayer = (Vector3.Distance(transform.position, _PLAYER.transform.position));

        //if (changeState)
        //{
        //    if (distToPlayer < maxDistFromPlayer)
        //    {
        //        inRange = true;
        //        state = EnemyState.Roam;
        //    }
        //    else
        //    {
        //        inRange = false;
        //        state = EnemyState.Chase;
        //    }
        //}


        //transform.position - _PLAYER.transform.position
        //transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        if (state == EnemyState.Roam || state == EnemyState.Chase)
            transform.LookAt(new Vector3(_PLAYER.transform.localPosition.x, transform.position.y, _PLAYER.transform.localPosition.z));
        else if (state == EnemyState.Attack)
            transform.LookAt(lastPlayerLocation);
        //orientation.LookAt(new Vector3(_PLAYER.transform.localPosition.x, transform.position.y, _PLAYER.transform.localPosition.z));
        //transform.forward = Vector3.Slerp(transform.forward, _PLAYER.transform.position, Time.deltaTime); // * rotationSpeed

        switch (state)
        {
            case EnemyState.Roam:
                Roam();
                break;
            case EnemyState.Chase:
                Chase();
                break;
        }

        //if (Input.GetKeyDown("space"))
        //    Jump();
            
    }

    void Roam()
    {
        if (state != EnemyState.Roam)
         return;

        //attackBlocked = false;

        anim.SetBool("Chase", false);
        agent.speed = walkSpeed;
        attacking = false;

        //Keeps enemy in range of player
        if (changeState)
            if (distToPlayer > maxDistFromPlayer)
                state = EnemyState.Chase;

        //Searches for location to move towards
        if (!walkpointSet)
            SearchForDest();
        //Move towards selected location
        if (walkpointSet)
            agent.SetDestination(destPoint);
        //Restarts search for location if close enough to selected location
        if (Vector3.Distance(transform.position, destPoint) < maxDistFromDest)
            RollForAttack();
    }

    void SearchForDest()
    {
        float x = UnityEngine.Random.Range(-moveRangeX, moveRangeX);
        float z = UnityEngine.Random.Range(-moveRangeZ, moveRangeZ);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Vector3.Distance(destPoint, _PLAYER.transform.position) < maxDistFromPlayer && Vector3.Distance(destPoint, _PLAYER.transform.position) > minDistFromPlayer)
            if(Physics.Raycast(destPoint, Vector3.down, groundMask))
            {
                walkpointSet = true;
            }
    }

    void RollForAttack()
    {
        int randomNumber = UnityEngine.Random.Range(1, _EM.attackChance + 1);

        if (randomNumber == _EM.attackChance && _EM.enemiesAttacking.Count <= _EM.maxAttacking)
        {
            attacking = true;

            _EM.enemiesAttacking.Add(this.gameObject);

            if (distToPlayer <= attackDistance)
                Attack();
            else
                state = EnemyState.Chase;
        }
        else
            walkpointSet = false;
    }

    void Chase()
    {
        if (state != EnemyState.Chase)
            return;

        anim.SetBool("Chase", true);
        agent.speed = runSpeed;

        walkpointSet = false;
        agent.SetDestination(_PLAYER.transform.position);

        switch (attacking)
        {
            case true:
                if (distToPlayer <= attackDistance)
                    Attack();
                break;
            case false:
                if (distToPlayer < maxDistFromPlayer)
                    state = EnemyState.Roam;
                break;
        }
    }

    void Attack()
    {
        lastPlayerLocation = new Vector3(_PLAYER.transform.localPosition.x, transform.position.y, _PLAYER.transform.localPosition.z);

        attackProperties.attackLanded = false;
        agent.speed = 0f;
        anim.applyRootMotion = true;
        state = EnemyState.Attack;
        anim.SetTrigger("atk" + UnityEngine.Random.Range(1, 3));
    }

    public void TakeDamage(int _damage, int _anim = 1)
    {
        health -= _damage;
        healthBar.UpdateHealthBar(health, maxHealth);

        if (health <= 0 && canDie)
        {
            StopAllCoroutines();
            StartCoroutine(Die());
        }
            
        else
        {
            state = EnemyState.Damage;

            agent.speed = 0f;
            agent.enabled = false;
            rb.isKinematic = false;

            anim.SetTrigger("Damage" + _anim);
        }
    }

    public void Knockback(float _knockbackXZ, float _knockbackY)
    {
        ////Determine direction of knockback
        //Vector3 knockbackDirection = transform.position - _PLAYER.transform.position;
        ////Resets velocity to prevent knockback compounding
        //rb.velocity = new Vector3(0, 0, 0);
        ////Apply knockback
        //rb.AddForce(knockbackDirection.x * _knockbackXZ, knockbackDirection.y * _knockbackY, knockbackDirection.z * _knockbackXZ, ForceMode.Impulse);
    }

    void Jump()
    {
        rb.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
    }

    IEnumerator PositionCheck()
    {
        position1 = transform.position;
        yield return new WaitForSeconds(0.1f);
        position2 = transform.position;
        Vector3 targetDir = position1 - position2;
        enemyAngle = Vector3.Angle(targetDir, transform.forward);
        StartCoroutine(PositionCheck());
    }

    public void RemoveFromAttackList()
    {
        _EM.enemiesAttacking.Remove(this.gameObject);
    }

    IEnumerator Die()
    {
        state = EnemyState.Die;

        healthBar.gameObject.SetActive(false);

        anim.SetTrigger("PlayerDead");
        GetComponent<MannequinExplode>().Explode();
        //_AM.PlaySound(_AM.GetDeathSound(), audioSource, 0f, 0.5f);
        _AM.PlayDeathSound(audioSource, 0.35f);
        agent.speed = 0;
        GetComponent<Collider>().enabled = false;
        OnEmemyDie?.Invoke(this.gameObject);

        if (_PLAYER.health <= lowPlayerHealth)
            SpawnHealthPickup();

        yield return new WaitForSeconds(0.5f);

        Destroy(this.gameObject);
    }

    void SpawnHealthPickup()
    {
        int randomNumber = UnityEngine.Random.Range(1, _EM.hpSpawnChance + 1);

        if (randomNumber == _EM.hpSpawnChance)
            Instantiate(healthPickUp, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
    }

    void PlayerIsDead()
    {
        state = EnemyState.PlayerDead;
        agent.speed = 0;
        anim.SetTrigger("PlayerDead");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary"))
            onBoundary = true;

        if (other.CompareTag("OutOfBounds"))
            Die();

        //if (other.CompareTag("Enemy") && state == EnemyState.Damage)
        //{

        //    Enemy enemy = other.GetComponent<Enemy>();
        //    Rigidbody enemyRB = other.attachedRigidbody;

        //    enemy.TakeDamage(1);
        //    //enemy.Knockback(attack.knockbackXZ, attack.knockbackY);

        //    //Determine direction of knockback
        //    Vector3 knockbackDirection = enemyRB.transform.position - transform.position;
        //    //Resets velocity to prevent knockback compounding
        //    //enemyRB.isKinematic = false;
        //    enemyRB.velocity = new Vector3(0, 0, 0);
        //    //Apply knockback
        //    enemyRB.AddForce(knockbackDirection.x * 200f, knockbackDirection.y * 0f, knockbackDirection.z * 200f);
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boundary"))
            onBoundary = false;
    }

    private void OnEnable()
    {
        PlayerMovement.PlayerDeath += PlayerIsDead;
    }

    private void OnDisable()
    {
        PlayerMovement.PlayerDeath -= PlayerIsDead;
    }
}
