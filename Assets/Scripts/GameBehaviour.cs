using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    protected static EnemyManager _EM           { get { return EnemyManager.INSTANCE; } }
    protected static PlayerMovement _PLAYER     { get { return PlayerMovement.INSTANCE; } }
}
