using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RecenterCamera : MonoBehaviour
{
    private CinemachineFreeLook cinCam;

    void Start()
    {
        cinCam = GetComponent<CinemachineFreeLook>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            cinCam.m_RecenterToTargetHeading.m_enabled = true;
            cinCam.m_YAxisRecentering.m_enabled = true;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            cinCam.m_RecenterToTargetHeading.m_enabled = false;
            cinCam.m_YAxisRecentering.m_enabled = false;
        }
    }
}
