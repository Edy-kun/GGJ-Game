using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class EnemyManager
{
    public DamagableEnemy Enemies;
    public EnemyManager()
    {
        Enemies = new GameObject().AddComponent<DamagableEnemy>();
        enemyPositions = new List<DamagableEnemy>();
        for (var i = 0; i < 10; i++)
        {
            var a = Object.Instantiate(Enemies, new Vector3(Random.value, 0, Random.value), Quaternion.identity);
            enemyPositions.Add(a);
        }
        
        
    }

  
    public List<DamagableEnemy> enemyPositions;
    private float tollerance = 0.5f;

    public List<DamagableEnemy> CheckHit(Vector3 orig, float angle)
    {
        return enemyPositions.Where(item => Math.Abs(Vector3.Angle(orig, item.pos) - angle) < tollerance).ToList();
    }
}