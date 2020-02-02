using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class RedNeckBomber : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform[] points;

    [SerializeField] private float remainingDistance = 1f, spawnAfter = 2f;

    [SerializeField] private GameObject minePrefab;

    private int destPoint = 0;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GotoNextPoint();

        InvokeRepeating("ThrowRandomMine", 5f, 5f);

    }

    // Update is called once per frame
    void Update()
    {
        UpdateAgent();
    }

    private void ThrowRandomMine()
    {
        StartCoroutine(Throw());
    }

    private IEnumerator Throw()
    {
        animator.Play("Throw");
        yield return new WaitForSeconds(0.6f);
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y * (60f), transform.position.z);
        Instantiate(minePrefab, spawnPos - transform.forward * spawnAfter, Quaternion.identity);
    }

    private void UpdateAgent()
    {
        if (!agent.pathPending && agent.remainingDistance < remainingDistance)
        {
            GotoNextPoint();
        }
    }

    private void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < points.Length; i++)
        {
          //  Gizmos.color = Color.yellow;
            //Handles.Label(points[i].position, points[i].name);
           // Gizmos.DrawSphere(points[i].position, 0.25f);
        }
    }
}
