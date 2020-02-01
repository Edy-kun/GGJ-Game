using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

public class SensorResetHovercraft : MonoBehaviour
{
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] private Vector3 _respawnOffset = Vector3.up * 5;
    [SerializeField] private float _respawnTime = 3f;
    [SerializeField] private float _flipExplosiveForce;
    [SerializeField] private float _flipExplosiveRadius;


    [SerializeField] private InputActionAsset _inputActionAsset;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Flip();
        }
    }

    public void Flip()
    {
        Debug.Log("flipping!");
        _rigidbody.AddExplosionForce(_flipExplosiveForce,
            _rigidbody.transform.position - UnityEngine.Random.insideUnitSphere - Vector3.down,
            _flipExplosiveRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Terrain"))
        {
            StartCoroutine(Reset());
        }
    }

    private IEnumerator Reset()
    {
        var position = _rigidbody.transform.position;
        var oldPosition = position;
        var newPosition = position + _respawnOffset;
        _rigidbody.transform.position = newPosition;
        var forward = Vector3.Cross(_rigidbody.transform.forward, Vector3.up);
        _rigidbody.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        _rigidbody.isKinematic = true;
        var startTime = Time.time;
        while (Time.time < startTime + _respawnTime)
        {
            yield return null;
        }

        _rigidbody.isKinematic = false;
    }
}