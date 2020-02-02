using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PickUp : MonoBehaviour
{
    [SerializeField] private PickUpProfile _contains;
    public Action<PickUp> OnPickedUp { get; set; }

    private void Awake()
    {
        if (_contains != null)
        {
            SetProfile(_contains);
        }
    }

    public void SetProfile(PickUpProfile contain)
    {
        _contains = contain;
        Instantiate(_contains.GameObject, transform);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"entered {other.name}");
        var collidedWith = other.GetComponent<ICanPickUp>();
        if (collidedWith == null)
            return;
        if (collidedWith.TryPickUp(_contains))
        {
            OnPickedUp?.Invoke(this);
            Destroy(gameObject);
        }
    }
}