using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : GameBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Rigidbody rb;

    public float rotationSpeed;

    void Start()
    {
        ToggleCursorLockState();
    }

    private void Update()
    {
        if (Input.GetKeyDown("1"))
            ToggleCursorLockState();
    }

    void FixedUpdate()
    {
        // rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // rotate player object
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero && _PLAYER.state == PlayerState.Idle)
            player.forward = Vector3.Slerp(player.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
    }

    void ToggleCursorLockState()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.None;
        else if (Cursor.lockState == CursorLockMode.None)
            Cursor.lockState = CursorLockMode.Locked;
    }
}
