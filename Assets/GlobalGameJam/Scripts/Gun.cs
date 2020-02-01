using System;
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

    public Action OnControlEnd { get; set; }

    public void StartControl()
    {
        throw new NotImplementedException();
    }

    event Action IControlled.OnControlEnd
    {
        add => throw new NotImplementedException();
        remove => throw new NotImplementedException();
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
        OnControlEnd?.Invoke();
    }
}
