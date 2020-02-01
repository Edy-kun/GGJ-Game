using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemyPlacement : MonoBehaviour
{
    [SerializeField] private GameObject crocPrefab;
    [SerializeField] private float minSpawnRange, maxSpawnRange, minSpawnTime, maxSpawnTime;
    [SerializeField] private int maxEnemies;

    private List<GameObject> allEnemiesInScene = new List<GameObject>();
    private int numberOfEnemies;

    private bool isBusyWithSpawning;

    private void Start()
    {
        
    }

    private void Update()
    {
        LastTimeInRange();
        if (!isBusyWithSpawning)
        {
            if (numberOfEnemies < maxEnemies)
            {
                StartCoroutine(SpawnEnemy());
            }
        }
    }

    private void LastTimeInRange()
    {
        for (var i = 0; i < allEnemiesInScene.Count; i++)
        {
            if (Vector3.Distance(allEnemiesInScene[i].transform.position, transform.position) > maxSpawnRange)
            {
                GameObject deleteObject = allEnemiesInScene[i];
                allEnemiesInScene.Remove(deleteObject);
                Destroy(deleteObject);
            }
        }
    }


    IEnumerator SpawnEnemy()
    {
        isBusyWithSpawning = true;
        GameObject croc = Instantiate(crocPrefab, transform.position + RandomBetweenRadius3D(minSpawnRange, maxSpawnRange), Quaternion.identity);
        allEnemiesInScene.Add(croc);
        numberOfEnemies = allEnemiesInScene.Count;
        yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
        isBusyWithSpawning = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minSpawnRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxSpawnRange);
    }

    Vector3 RandomBetweenRadius3D(float minRad, float maxRad)
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
}
