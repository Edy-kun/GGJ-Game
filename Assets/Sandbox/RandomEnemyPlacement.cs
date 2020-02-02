using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEnemyPlacement : MonoBehaviour
{
    [SerializeField] private bool isBandit;
    [SerializeField] private TurretAI[] enemyPrefabs;
    [SerializeField] private float minSpawnRange, maxSpawnRange, minSpawnTime, maxSpawnTime;
    [SerializeField] private int maxEnemies;

    [SerializeField] private Transform _hoverCraft;

    public readonly List<TurretAI> allEnemiesInScene = new List<TurretAI>();

    private bool spawnEnemies = true;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        LastTimeInRange();
        while(spawnEnemies)
        {
            if (allEnemiesInScene.Count < maxEnemies)
            {
                SpawnEnemy(enemyPrefabs[Random.Range(0,enemyPrefabs.Length)]);
                
                yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            }

            yield return null;
        }
    }

    private void LastTimeInRange()
    {
        for (var i = 0; i < allEnemiesInScene.Count; i++)
        {
            if (Vector3.Distance(allEnemiesInScene[i].transform.position, _hoverCraft.position) > maxSpawnRange)
            {
                var x = allEnemiesInScene[i];
                Destroy(x);
            }
        }
    }


    void SpawnEnemy(TurretAI prefab)
    {
        var enem = Instantiate(prefab, _hoverCraft.position + RandomBetweenRadius3D(minSpawnRange, maxSpawnRange), Quaternion.identity);
        enem.ListOfEnemies = allEnemiesInScene;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_hoverCraft.position, minSpawnRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_hoverCraft.position, maxSpawnRange);
    }

    static Vector3 RandomBetweenRadius3D(float minRad, float maxRad)
    {
        float diff = maxRad - minRad;
        Vector3 point = Vector3.zero;
        while (point == Vector3.zero)
        {
            point = Random.insideUnitSphere;
        }
        point = point.normalized * (Random.value * diff + minRad);
        point.y = 0;
        return point;
    }
    
    private float tollerance = 0.5f;

    public List<TurretAI> CheckHit(Vector3 orig, float angle)
    {
        return allEnemiesInScene.Where(item => Math.Abs(Vector3.Angle(orig, item.transform.position) - angle) < tollerance).ToList();
    }
}

