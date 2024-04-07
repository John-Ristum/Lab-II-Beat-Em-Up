using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStart : GameBehaviour
{
    public GameObject bridge;

    public List<Transform> spawnPoints;
    bool combatStarted;

    private void Start()
    {
        combatStarted = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !combatStarted)
        {
            combatStarted = true;

            bridge.GetComponent<ActivateBridge>().EnableEvent();

            _EM.spawnPoints = spawnPoints;
            _EM.SpawnEnemies();
        }
    }
}
