using System;
using UnityEngine;

public class DamagableEnemy : MonoBehaviour, IDamageable
{
    public Vector3 pos;
    public Action<int> OnKilled;
    public int Value;
    public int Health { get; set; }
    public void TakeDamage(int dmg)
    {
        Health -= dmg;
        if (Health < 0)
        {
            Die();
        }
    }

    public void Die()
    {
        OnKilled?.Invoke(Value);
    }
   
}