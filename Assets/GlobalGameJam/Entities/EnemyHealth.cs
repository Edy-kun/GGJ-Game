using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int totalHealth;
    public int currentHealth;

    [SerializeField] private GameObject explosion;

    private void Start()
    {
        currentHealth = totalHealth;
    }

    private void Update()
    {
        if (currentHealth == 0 || currentHealth < 0)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void DoDamage(int damage)
    {
        currentHealth -= damage;
    }
}
