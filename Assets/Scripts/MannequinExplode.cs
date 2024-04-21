using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannequinExplode : MonoBehaviour
{
    public GameObject[] bodyParts;

    public void Explode()
    {
        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].GetComponent<Destructible>().DestroyObject();
        }
    }
}
