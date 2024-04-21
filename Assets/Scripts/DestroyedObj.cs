using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedObj : MonoBehaviour
{
    float timer = 5;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(Random.Range(-50f, 50f), Random.Range(10f, 500f), Random.Range(-50f, 50f));
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
            Destroy(this.gameObject);
    }
}
