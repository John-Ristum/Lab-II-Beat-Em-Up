using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RecenterCamera : GameBehaviour
{
    private CinemachineFreeLook cinCam;

    void Start()
    {
        cinCam = GetComponent<CinemachineFreeLook>();
    }

    void Update()
    {
        if (_PLAYER.inCutscene == true)
            return;

        if (Input.GetButtonDown("RecenterCam"))
        {
            cinCam.m_RecenterToTargetHeading.m_enabled = true;
            cinCam.m_YAxisRecentering.m_enabled = true;
        }
        else if (Input.GetButtonUp("RecenterCam"))
        {
            cinCam.m_RecenterToTargetHeading.m_enabled = false;
            cinCam.m_YAxisRecentering.m_enabled = false;
        }
    }
}
