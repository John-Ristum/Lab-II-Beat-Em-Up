using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : GameBehaviour
{
    public float OSTVolume = 1f;

    public void PlayMenuOST()
    {
        _AM.PlaySound(_AM.soundtrack, _AM.audioSource, 0.8f, OSTVolume);
    }
}
