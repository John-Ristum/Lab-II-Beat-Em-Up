using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public Transform[] spawnPoints;
    public GameObject[] enemyTypes;

    public List<GameObject> enemies;

    void Start()
    {
        SpawnEnemies();
    }

    void Update()
    {
        
    }

    void SpawnEnemies()
    {
        for (int i = 0; i <= spawnPoints.Length - 1; i++)
        {
            GameObject enemy = Instantiate(enemyTypes[0], spawnPoints[i].position, spawnPoints[i].rotation);
            enemies.Add(enemy);
        }
    }
}
