using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class RandomEnemyPlacement : ITickable
{
   
    
    private Settings _settings; 
    private Transform _hoverCraft;
    private TurretAI.Factory _factory;

    public readonly List<TurretAI> allEnemiesInScene = new List<TurretAI>();

    private bool spawnEnemies = true;
    private Collider _terrainCollider;

    public RandomEnemyPlacement(TurretAI.Factory factory, Settings settings, Boat hover,TerrainCollider col)
    {
        _terrainCollider = col;
        _factory = factory;
        _settings = settings;
        _hoverCraft = hover.transform;
    }


    private IEnumerator SpawnEnemies()
    {
        LastTimeInRange();
        while(spawnEnemies)
        {

            if (allEnemiesInScene.Count < _settings.maxEnemies)
            {
                
                SpawnEnemy();
                
                yield return new WaitForSeconds(Random.Range( _settings.minSpawnTime,  _settings.maxSpawnTime));
            }

            yield return null;
        }
    }

    private float lastSpawn = 0;
    private float spawnRate = .5f;
    public void Tick()
    {
        LastTimeInRange();
        if (!(lastSpawn + spawnRate < Time.realtimeSinceStartup) || allEnemiesInScene.Count >=  _settings.maxEnemies) 
            return;
        lastSpawn = Time.realtimeSinceStartup;
        SpawnEnemy();

    }

    private void LastTimeInRange()
    {
        foreach (var t in allEnemiesInScene)
        {
            if (Vector3.Distance(t.transform.position, _hoverCraft.position) >  _settings.maxSpawnRange)
            {
                var x = t;
                Object.Destroy(x.gameObject);
            }
        }
    }


    void SpawnEnemy()
    {
        
        var position = _hoverCraft.position + RandomBetweenRadius3D( _settings.minSpawnRange,  _settings.maxSpawnRange);

        if (_terrainCollider
            .Raycast(new Ray(position + Vector3.up * 100, Vector3.down), out var hitInfo, 2000))
        {
            position = hitInfo.point;
        }

        var enem = _factory.Create();
        enem.transform.position = new Vector3(position.x,0,position.z);// Object.Instantiate(prefab,new Vector3(position.x, 0, position.z), Quaternion.identity);
        enem.ListOfEnemies = allEnemiesInScene;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_hoverCraft.position,  _settings.minSpawnRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_hoverCraft.position,  _settings.maxSpawnRange);
    }

    static Vector3 RandomBetweenRadius3D(float minRad, float maxRad)
    {
        float diff = maxRad - minRad;
        var point = Random.onUnitSphere;
        point = point * (Random.value * diff + minRad);
        point.y = 0;
        return point;
    }
    
    private float tollerance = 0.1f;

    public IEnumerable<TurretAI> CheckHit(Vector3 orig, Vector3 forward)
    {
        forward.y = 0;
        orig.y = 0;
        foreach (var item in allEnemiesInScene)
        {
            var itemPosition = item.transform.position;
            itemPosition.y = 0;
            var direction = (itemPosition - orig).normalized;
            var dot = Vector3.Dot(direction, forward);
            Debug.DrawRay(orig, forward);
            if( dot > 1-tollerance)
                yield return (item);
        }
    }
 
    [Serializable]
    public class Settings
    {
        public float
            minSpawnRange,
            maxSpawnRange,
            minSpawnTime,
            maxSpawnTime;

        public int maxEnemies;
    }

    
}

