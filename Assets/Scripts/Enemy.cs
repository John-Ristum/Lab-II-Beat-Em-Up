using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Roam, Chase, Attack, Damage, Die}
public class Enemy : GameBehaviour
{
    [Header("General")]
    public EnemyState state;
    public float health = 100;
    Animator anim;
    NavMeshAgent agent;
    [SerializeField] LayerMask ground;
    public bool changeState;
    Rigidbody rb;
    public Vector3 moveDir;
    int randomNumber;
    float distToPlayer;
    public bool attacking;


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
    [SerializeField] float waitTime = 1f;
    //public bool patrolSet = true;           //Used in animator to revert to patrol state once animation is done

    [Header("Chase State")]
    public float runSpeed = 6f;
    public float attackDistance = 1f;


    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
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
        transform.LookAt(new Vector3(_PLAYER.transform.localPosition.x, transform.position.y, _PLAYER.transform.localPosition.z));

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
        float x = Random.Range(-moveRangeX, moveRangeX);
        float z = Random.Range(-moveRangeZ, moveRangeZ);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Vector3.Distance(destPoint, _PLAYER.transform.position) < maxDistFromPlayer && Vector3.Distance(destPoint, _PLAYER.transform.position) > minDistFromPlayer)
            if(Physics.Raycast(destPoint, Vector3.down, ground))
            {
                walkpointSet = true;
            }
    }

    void RollForAttack()
    {
        randomNumber = Random.Range(0, 5);

        if (randomNumber >= 4)
        {
            attacking = true;

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
        agent.speed = 0f;
        anim.applyRootMotion = true;
        state = EnemyState.Attack;
        anim.SetTrigger("atk" + Random.Range(1, 3));
        randomNumber = 0;
    }

    public void TakeDamage(int _damage)
    {
        state = EnemyState.Damage;
        agent.speed = 0f;
        anim.SetTrigger("Damage");
        health -= _damage;
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
}
