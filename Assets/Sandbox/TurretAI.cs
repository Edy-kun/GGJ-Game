using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class TurretAI : MonoBehaviour, IDamageable
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform partToRotate, gunToRotate;

    [SerializeField] private float minFollowRange, maxFollowRange, minTurretRange, maxTurretRange, turretTurnSpeed, collisionRange;
    [SerializeField] private string boatTag;

    [SerializeField] private float fireDelta;

    [SerializeField] private GameObject ps, bullet;

    private Transform target;
    private float cachedSpeed, nextFire = 0.4f, myTime = 0.0f;
    private AudioSource audioSource;
    public List<TurretAI> ListOfEnemies { get; set; }

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        InvokeRepeating("UpdateTarget", 0.0f, 0.5f);
        cachedSpeed = agent.speed;
        ListOfEnemies.Add(this);
    }

    private void Update()
    {
        if (target == null)
            return;

        //Collision with other animals
        RayCastUpdate();

        //Range
        if (DetectPlayerInRange(minFollowRange, maxFollowRange, target) == true)
        {
            agent.destination = target.position;
        }

        //Turret
        if (DetectPlayerInRange(minTurretRange, maxTurretRange, target) == true)
        {
            RotateTowardsTarget();
            RotateGunTowardsTarget();
            Fire();
        }
    }

    private void UpdateTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(boatTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestTarget = null;
        foreach (GameObject target in targets)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget < shortestDistance)
            {
                shortestDistance = distanceToTarget;
                nearestTarget = target;
            }
        }

        if (nearestTarget != null && shortestDistance <= Mathf.Infinity)
        {
            target = nearestTarget.transform;
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
        if (target == null)
        {
            return;
        }

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turretTurnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    private void RotateGunTowardsTarget()
    {
        if (target == null)
        {
            return;
        }

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(gunToRotate.rotation, lookRotation, Time.deltaTime * turretTurnSpeed).eulerAngles;
        gunToRotate.localRotation = Quaternion.Euler(rotation.x - 0.1f, 0f, 0f);
    }

    private void Fire()
    {
        myTime = myTime + Time.deltaTime;

        RaycastHit hit;
        if (Physics.Raycast(partToRotate.position,partToRotate.TransformDirection(Vector3.forward), out hit, maxTurretRange) && myTime > nextFire)
        {
            nextFire = myTime + fireDelta;

            Vector3 forward = partToRotate.TransformDirection(Vector3.forward) * maxTurretRange;
            Debug.DrawRay(partToRotate.position, forward, Color.yellow);

            GameObject shootBullet;
            shootBullet = Instantiate(bullet, ps.transform.position, partToRotate.rotation);
            shootBullet.GetComponent<Rigidbody>().AddForce(shootBullet.transform.forward * 90f);
            Destroy(shootBullet,0.4f);
            GetComponent<AudioSource>().Play();
            nextFire = nextFire - myTime;
            myTime = 0.0f;
        }

    }

    private void RayCastUpdate()
    {

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

    private void OnDestroy()
    {
    
        ListOfEnemies.Remove(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxFollowRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxTurretRange);
    }

    private int _health = 100;

    public int Health
    {
        get => _health;
        set => _health = value;
    }

    public void TakeDamage(int dmg)
    {
        _health -= dmg;
        if (_health <= 0)
        {
            GameManager.Instance._team.Score += 10;
            Destroy(this.gameObject);
        }
    }
}
