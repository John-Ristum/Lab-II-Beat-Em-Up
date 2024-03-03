using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForEnemy : GameBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            _PLAYER.enemiesInRange.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
            _PLAYER.enemiesInRange.Remove(other.gameObject);
    }
}
