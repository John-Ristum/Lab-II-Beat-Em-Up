using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public static event Action AllEnemiesDead = null;

    public int attackChance = 5;
    public int maxAttacking = 3;

    public List<Transform> spawnPoints;
    public GameObject[] enemyTypes;

    public List<GameObject> enemies;
    public List<GameObject> enemiesAttacking;

    void Start()
    {
        //spawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawnPoints");
        //SpawnEnemies();
    }

    void Update()
    {
        
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i <= spawnPoints.Count - 1; i++)
        {
            GameObject enemy = Instantiate(enemyTypes[0], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
            enemies.Add(enemy);
        }
    }

    public void KillEnemy(GameObject _enemy)
    {
        enemies.Remove(_enemy);

        if(enemies.Count == 0)
            AllEnemiesDead();
    }

    private void OnEnable()
    {
        Enemy.OnEmemyDie += KillEnemy;
    }

    private void OnDisable()
    {
        Enemy.OnEmemyDie -= KillEnemy;
    }

    void testIDK()
    {
        
    }
}
