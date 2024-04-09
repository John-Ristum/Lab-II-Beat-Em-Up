using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBridge : MonoBehaviour
{
    public GameObject oldBoundary;
    public GameObject newBoundary;

    void ContinueLevel()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("ActivateBridge");

        if (oldBoundary != null && newBoundary != null)
        {
            newBoundary.SetActive(true);
            oldBoundary.SetActive(false);
        } 
    }

    public void EnableEvent()
    {
        EnemyManager.AllEnemiesDead += ContinueLevel;
    }

    private void OnDisable()
    {
        EnemyManager.AllEnemiesDead -= ContinueLevel;
    }
}
