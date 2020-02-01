using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurretAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform partToRotate;

    [SerializeField] private float minFollowRange, maxFollowRange, minTurretRange, maxTurretRange, turretTurnSpeed, collisionRange;
    [SerializeField] private string boatTag;

    private Transform player;
    private float cachedSpeed;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(boatTag).transform;
        agent.destination = player.position;
        cachedSpeed = agent.speed;
    }

    private void Update()
    {
        RayCastUpdate();

        //Range
        if (DetectPlayerInRange(minFollowRange, maxFollowRange, player) == true)
        {
            agent.destination = player.position;
        }

        //Turret
        if (DetectPlayerInRange(minTurretRange, maxTurretRange, player) == true)
        {
            RotateTowardsTarget();
        }
    }

    private bool DetectPlayerInRange(float minRange, float maxRange, Transform targetTransform)
    {
        bool inRange;
        float dist = Vector3.Distance(targetTransform.position, transform.position);
        if (dist > minRange && dist < maxRange)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }

        return inRange;
    }

    private void RotateTowardsTarget()
    {
        if (player == null)
        {
            return;
        }

        Vector3 dir = player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turretTurnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    private void RayCastUpdate()
    {
        //Debug Turret
        Vector3 forwardF = transform.TransformDirection(Vector3.forward) * maxFollowRange;
        Debug.DrawRay(transform.position, forwardF, Color.red);

        Vector3 forward = partToRotate.TransformDirection(Vector3.forward) * maxTurretRange;
        Debug.DrawRay(partToRotate.position, forward, Color.yellow);

        RaycastHit hit;
        if (Physics.Raycast(partToRotate.position, partToRotate.TransformDirection(Vector3.forward), out hit, collisionRange) || Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, collisionRange))
        {
            if (hit.transform.gameObject.tag == "Enemy")
            {
                agent.speed = 0f;
            }
        }
        else
        {
            agent.speed = cachedSpeed;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxFollowRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxTurretRange);
    }

}
