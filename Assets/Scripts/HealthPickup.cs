using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : GameBehaviour
{
    public int recoveryAmount = 10;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(0, 100, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _PLAYER.RecoverHealth(recoveryAmount);
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime * 1f);
    }
}
