using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShowSpawn : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Handles.Label(transform.position, gameObject.transform.name);
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
