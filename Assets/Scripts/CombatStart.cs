using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStart : GameBehaviour
{
    public GameObject bridge;
    public GameObject oldBridge;
    public GameObject oldAreaBoundary;

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

            if (bridge != null)
                bridge.GetComponent<ActivateBridge>().EnableEvent();
            if (oldBridge != null)
                oldBridge.GetComponent<Animator>().SetTrigger("CloseOldBridge");
            if (oldAreaBoundary != null)
                oldAreaBoundary.SetActive(true);

            _EM.spawnPoints = spawnPoints;
            _EM.SpawnEnemies();
        }
    }
}
