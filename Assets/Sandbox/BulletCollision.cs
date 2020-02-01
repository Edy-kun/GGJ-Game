using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    private int dmg = 50;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null) 
            return;
        damageable.TakeDamage(dmg);
        Destroy(gameObject);
    }
}
