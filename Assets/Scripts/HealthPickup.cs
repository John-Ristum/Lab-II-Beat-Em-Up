using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : GameBehaviour
{
    public int recoveryAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _PLAYER.RecoverHealth(recoveryAmount);
            Destroy(this.gameObject);
        }
    }
}
