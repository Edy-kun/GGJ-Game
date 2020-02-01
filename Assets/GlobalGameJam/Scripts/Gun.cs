using System;
using UnityEngine;

public class Gun : Device, IWeapon, IControlled
{
    public float fireRate { get; set; }
    public float _previousFire;
    private Element Cost;
 
    public void Shoot()
    {
        var time = Time.timeSinceLevelLoad;
        if (_previousFire < time + fireRate)
        {
            Boat.Inventory.TrySubstract(Cost);
            _previousFire = time;
            
        }
    }
    

    public void StartControl()
    {
        throw new NotImplementedException();
    }

    public event Action<IControlled> OnControlEnd;

    public void EndControl()
    {
        //cleanup here.
    }

    private void EndInteraction()
    {
        OnControlEnd?.Invoke(this);
    }
}
