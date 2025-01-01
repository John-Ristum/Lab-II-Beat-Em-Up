using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonCam : GameBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Rigidbody rb;

    public float rotationSpeed;


    public CinemachineBrain brain;
    public bool testBool;

    private void Start()
    {
        //Application.targetFrameRate = 60;
    }

    void FixedUpdate()
    {
        //For Old Player Controller
        //if (!testBool)
        //    brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
        //else
        //    brain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;


        // rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // rotate player object
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir.magnitude >= 0.1)
            if (inputDir != Vector3.zero && _PLAYER.state == PlayerState.Idle)
                player.forward = Vector3.Slerp(player.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
    }
}
