using UnityEngine;

public class PickUp : MonoBehaviour
{
    private Element _cointains;
    private void OnCollisionEnter(Collision other)
    {
        var collidedWith = other.transform.GetComponent<ICanPickUp>();
        if (collidedWith == null) 
            return;
        if (collidedWith.TryPickUp(_cointains))
        {
            Destroy(gameObject);
        }
    }
}