using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CinemachineBrain cineBrain;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
