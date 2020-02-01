using System;
using UnityEngine;

public class Gun : Device, IWeapon, IControlled, IReceiveInput
{
    public float fireRate { get; set; }
    public float _previousFire;
    private Element Cost;

    private float position;
    public Vector3 begin;
    public Vector3 end;
 
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

    public void Move(Vector3 move)
    {
        position += move.x * .1f;
        Mathf.Clamp(position, 0, 1);
        this.transform.position = Vector3.Lerp(begin, end, position);
    }

    public void Rotate(Vector3 rotate)
    {
        throw new NotImplementedException();
    }

    public void Yes()
    {
       Shoot();
    }

    public void No()
    {
        EndInteraction();
    }

    public void Interact()
    {
        throw new NotImplementedException();
    }
}
