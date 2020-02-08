using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class BulletCollision : MonoBehaviour, IPoolable<BulletCollision.Pool>//, IDisposable
{
    public int dmg;
    private Pool _pool;
    private float _startTime;
    public Rigidbody rb;


    private void Awake()
    {
      
    }

    private void OnTriggerEnter(Collider other){
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null) 
            return;
        damageable.TakeDamage(dmg);
        Remove();
    }

    private void Update()
    {
        if (_startTime + 0.5 < Time.realtimeSinceStartup)
        {
            Remove();
        }
    }

    private void Remove()
    {
      
        
        this.gameObject.SetActive(false);
        _pool?.Despawn(this);
    }


    public void OnDespawned()
    {
        this.gameObject.SetActive(false);
    }

    public void OnSpawned(Pool p1)
    {
       
        gameObject.SetActive(true);
        _pool = p1;
    }

    
    


    public class Pool : MemoryPool<BulletCollision>
    {
        protected override void OnSpawned(BulletCollision item)
        {
            base.OnSpawned(item);
            item.transform.position = Vector3.zero;
            item.OnSpawned(this);
            item.gameObject.SetActive(true);
            item._startTime= Time.realtimeSinceStartup;
        }

        protected override void OnDespawned(BulletCollision item)
        {
            base.OnDespawned(item);
            item.gameObject.SetActive(false);
            item.rb.velocity = Vector3.zero;
            item.rb.angularVelocity = Vector3.zero;
            item.rb.position= Vector3.zero;
            
        }
    }

}
