using UnityEngine;

public class PickUp : MonoBehaviour
{
    private PickUpProfile _contains;

    public void SetProfile(PickUpProfile contain)
    {
        _contains = contain;
        GameObject.Instantiate(_contains.GameObject);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        var collidedWith = other.transform.GetComponent<ICanPickUp>();
        if (collidedWith == null) 
            return;
        if (collidedWith.TryPickUp(_contains.Element))
        {
            Destroy(gameObject);
        }
    }
}