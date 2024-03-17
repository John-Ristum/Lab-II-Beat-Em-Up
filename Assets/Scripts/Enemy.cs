using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Roam, Chase, Attack, Damage, Die}
public class Enemy : GameBehaviour
{
    public EnemyState state;

    Animator anim;

    NavMeshAgent agent;
    [SerializeField] LayerMask ground;
    public bool changeState;

    //patrol
    Vector3 destPoint;
    public bool walkpointSet;
    public bool inRange;
    [SerializeField] float moveRange = 5;
    [SerializeField] float maxDistFromPlayer = 8;
    [SerializeField] float waitTime = 1f;
    //public bool patrolSet = true;           //Used in animator to revert to patrol state once animation is done

    float speed = 1.75f;

    Rigidbody rb;
    public Vector3 moveDir;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        //moveDir = agent.desiredVelocity.normalized;
        moveDir = new Vector3(transform.right.magnitude * agent.desiredVelocity.x, 0f, transform.forward.magnitude * agent.desiredVelocity.z);

        //anim.SetFloat("moveX", moveDir.x - _PLAYER.transform.position.x);
        anim.SetFloat("moveY", transform.forward.magnitude * moveDir.z);


        if (changeState)
        {
            if (Vector3.Distance(transform.position, _PLAYER.transform.position) < maxDistFromPlayer)
            {
                inRange = true;
                state = EnemyState.Roam;
            }
            else
            {
                inRange = false;
                state = EnemyState.Chase;
            }
        }


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
    }

    void Roam()
    {
        //if (state != EnemyState.Roam)
           // return;

        //Searches for location to move towards
        if (!walkpointSet)
            SearchForDest();
        //Move towards selected location
        if (walkpointSet)
            agent.SetDestination(destPoint);
        //Restarts search for location if close enough to selected location
        if (Vector3.Distance(transform.position, destPoint) < 1)
            walkpointSet = false;
    }

    void SearchForDest()
    {
        float x = Random.Range(-moveRange, moveRange);
        float z = Random.Range(-moveRange, moveRange);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Vector3.Distance(destPoint, _PLAYER.transform.position) < maxDistFromPlayer)
            if(Physics.Raycast(destPoint, Vector3.down, ground))
            {
                walkpointSet = true;
            }
    }

    void Chase()
    {
        //if (state != EnemyState.Chase)
            //return;

        walkpointSet = false;
        agent.SetDestination(_PLAYER.transform.position);
    }
}
