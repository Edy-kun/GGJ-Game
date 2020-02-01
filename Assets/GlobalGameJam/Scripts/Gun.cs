using UnityEngine;

public class Gun : Device, IWeapon, IControlled
{
    public float fireRate { get; set; }
    public float _previousFire;
 
    public void Shoot()
    {
        var time = Time.timeSinceLevelLoad;
        if (_previousFire < time + fireRate)
        {
            _previousFire = time;
            
            
        }
        
    }

    public void Rotate(Vector2 rotation)
    {
        throw new System.NotImplementedException();
    }

    public void Move(Vector2 movement)
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        Shoot();
    }

    public void EndInteraction()
    {
        throw new System.NotImplementedException();
    }
}
