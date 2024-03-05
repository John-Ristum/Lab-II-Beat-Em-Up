using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRBtoCC : MonoBehaviour
{
    Rigidbody rb;
    CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("9"))
            ActivateRB();
        if (Input.GetKeyDown("0"))
            ActivateCC();
    }

    void ActivateRB()
    {
        rb.isKinematic = false; // Activates Rigidbody
        cc.enabled = false;
    }

    void ActivateCC()
    {
        rb.isKinematic = true; // Deactivates Rigidbody
        cc.enabled = true;
    }
}
