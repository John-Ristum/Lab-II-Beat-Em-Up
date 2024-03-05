using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVelocity : MonoBehaviour
{
    Rigidbody rb;
    public Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = rb.velocity;

        if (Input.GetKeyDown("2"))
            ToggleY();
    }

    void ToggleY()
    {
        if (rb.constraints != RigidbodyConstraints.FreezePositionY)
            rb.constraints = RigidbodyConstraints.FreezePositionY;
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }  
    }
}
