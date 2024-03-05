using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded;

    public float fallRate = 0.5f;

    Animator anim;
    Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //anim = GetComponent<Animator>();
        //anim.SetBool("isGrounded", true);
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if we are touching the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //if (!isGrounded)
        //    //anim.SetBool("isGrounded", false);
        //else
        //    //anim.SetBool("isGrounded", true);
    }

    public void HitStun()
    {
        Debug.Log("Worked");
        StopAllCoroutines();
        StartCoroutine(FreezeY());
        FreezeY();
    }

    IEnumerator FreezeY()
    {
        Debug.Log("Enter");
        rb.constraints = RigidbodyConstraints.FreezePositionY;

        yield return new WaitForSeconds(fallRate);

        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        Debug.Log("Exit");
    }
}
