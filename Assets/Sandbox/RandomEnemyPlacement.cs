using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemyPlacement : MonoBehaviour
{
    [SerializeField] private GameObject crocPrefab;
    [SerializeField] private float minSpawnRange, maxSpawnRange;

    private void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minSpawnRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minSpawnRange);
    }
}
