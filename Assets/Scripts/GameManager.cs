using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum GameRank { S, A, B, C, D}

public class GameManager : Singleton<GameManager>
{
    public GameRank rank;
    public CinemachineBrain cineBrain;
    public float OSTVolume = 1f;
    public bool isTiming;
    public float timer;
    public int damageRecieved;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        isTiming = true;

        _AM.PlaySound(_AM.soundtrack, _AM.audioSource, 0.8f, OSTVolume);

        if (Cursor.lockState == CursorLockMode.None)
            Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTiming)
            timer += Time.deltaTime;
    }

    public void ToggleCursorLockState()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.None;
        else if (Cursor.lockState == CursorLockMode.None)
            Cursor.lockState = CursorLockMode.Locked;
    }

    public void CamUpdateLate()
    {
        if (cineBrain.m_UpdateMethod == CinemachineBrain.UpdateMethod.FixedUpdate)
            cineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
    }

    public void CamUpdateFixed()
    {
        if (cineBrain.m_UpdateMethod == CinemachineBrain.UpdateMethod.LateUpdate)
            cineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
    }

    public void CalculateRank()
    {
        Debug.Log("Rank Calculated");
        if (timer <= 91f && damageRecieved <= 0)
        {
            rank = GameRank.S;
            Debug.Log("Rank S");
            return;
        }
        else if (timer <= 121f && damageRecieved <= 25)
        {
            rank = GameRank.A;
            Debug.Log("Rank A");
            return;
        }
        else if (timer <= 151f && damageRecieved <= 50)
        {
            rank = GameRank.B;
            Debug.Log("Rank B");
            return;
        }
        else if (timer <= 181f && damageRecieved <= 80)
        {
            rank = GameRank.C;
            Debug.Log("Rank C");
            return;
        }
        else
        {
            rank = GameRank.D;
            Debug.Log("Rank D");
            return;
        }
    }
}
