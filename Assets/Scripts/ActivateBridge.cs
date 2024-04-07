using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBridge : MonoBehaviour
{
    void ContinueLevel()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("ActivateBridge");
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
