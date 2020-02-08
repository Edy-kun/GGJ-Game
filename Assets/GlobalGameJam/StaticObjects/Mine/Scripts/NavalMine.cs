using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavalMine : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        Destroy(gameObject, 60f);
    }

    private void Update()
    {
        if (!rb)
        {
            if (rb.velocity.magnitude < 0.1f)
            {
                Destroy(rb);
                col.isTrigger = true;
            }
        }
    }
}
