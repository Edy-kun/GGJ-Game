using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovercraftTracker : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _anglePerSecond;

    public void LateUpdate()
    {
        var targetPosition = _target.position;
        targetPosition.y = 0;
        var goalPosition =  targetPosition + _target.TransformDirection(_offset);
        var position = transform.position;
        var currentPosition = position;

        var direction = (goalPosition - currentPosition);
        position =
            currentPosition + direction.normalized * Mathf.Min(direction.magnitude, _moveSpeed * Time.deltaTime);
        transform.position = position;
        var goalRotation = Quaternion.LookRotation(targetPosition - position, Vector3.up);
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, goalRotation, _anglePerSecond * Time.deltaTime);

    }
}
