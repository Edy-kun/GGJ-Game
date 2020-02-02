using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepHoverCraftInPlayableArea : MonoBehaviour
{
    public Rigidbody rigidbody;
    public bool _playerInBounds = true;
    private Vector3 _originalPosition;

    public void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == rigidbody)
        {
            //Debug.Log("Player in bounds");
            _playerInBounds = true;
            _originalPosition = rigidbody.transform.position;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == rigidbody && _playerInBounds)
        {
            rigidbody.transform.position = _originalPosition;
        }
    }
}
