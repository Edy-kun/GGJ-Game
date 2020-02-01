using UnityEngine;

public class Player : MonoBehaviour, ICanPickUp
{
    public Element holds;
    public IWeapon Weapon;
    private Vector3 _aimDirection;

    
    void Rotate(Vector3 aimDir)
    {
        _aimDirection += aimDir;
    }
    
    public void Shoot()
    {
        
    }


    public bool TryPickUp(Element contains)
    {
        if (holds != null)
            return false;

        holds = contains;
        return true;
    }
}