using UnityEngine;

public class Gun : Device, IWeapon
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
}