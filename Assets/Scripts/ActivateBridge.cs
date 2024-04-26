using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBridge : GameBehaviour
{
    public float animWaitTime = 2f;
    public GameObject cutsceneCam;

    public GameObject oldBoundary;
    public GameObject newBoundary;

    void OpenNextArea()
    {
        StartCoroutine(StartCutscene(animWaitTime));

        if (oldBoundary != null && newBoundary != null)
        {
            newBoundary.SetActive(true);
            oldBoundary.SetActive(false);
        } 
    }

    IEnumerator StartCutscene(float _waitTime)
    {
        _PLAYER.inCutscene = true;
        _PLAYER.anim.SetFloat("movementSpeed", 0f);

        yield return new WaitForSeconds(1f);

        cutsceneCam.SetActive(true);

        yield return new WaitForSeconds(_waitTime);

        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("ActivateBridge");
    }

    IEnumerator EndCutscene(float _waitTime)
    {
        yield return new WaitForSeconds(1f);

        cutsceneCam.SetActive(false);

        yield return new WaitForSeconds(_waitTime);

        _PLAYER.inCutscene = false;
    }

    public void EnableEvent()
    {
        EnemyManager.AllEnemiesDead += OpenNextArea;
    }

    private void OnDisable()
    {
        EnemyManager.AllEnemiesDead -= OpenNextArea;
    }
}
