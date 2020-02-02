using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Gun : Device, IWeapon, IControlled, IReceiveInput
{
    public float fireRate
    {
        get => 0.05f;
        set { fireRate = value; }
    }

    public float _previousFire;
    private Element Cost;

    private float position =.5f;
    public Transform begin;
    public Transform end;

    public Vector3 AimDirMax;
    public Vector3 AimDirMin;
    public Transform TurrentCart;
    public Transform Turret;
    private Vector3 turrentCartStartPos;
    public Vector3 shotDir;
    public AudioClip shotsound;
    public GameObject bullet;
    
    protected override void Awake()
    {
        base.Awake();
        turrentCartStartPos = TurrentCart.transform.localPosition;
    }
    
    public override List<Element> GetRequiredItem()
    {
        return new List<Element> {config.RequiredElemnt};
    }

    public void Shoot()
    {
        var time = Time.timeSinceLevelLoad;
        if (_previousFire +fireRate < time)
        {
            
            Boat?.Inventory.TrySubstract(Cost);
            Fire();
            _previousFire = time;
            
        }
    }

    private float maxTurretRange = 15f;
    private Vector3 bulletOffset = new Vector3(0, 0.2f, 0);

    private void Fire()
    {


        Vector3 forward = Turret.forward * maxTurretRange;
        Debug.DrawRay(Turret.position, forward, Color.yellow);

        GameObject shootBullet;
        shootBullet = Instantiate(bullet, Turret.position+bulletOffset, Turret.rotation);
        
        shootBullet.GetComponent<Rigidbody>().AddForce(Turret.TransformDirection(shotDir) * 90f);
        Destroy(shootBullet, 0.4f);
        audioSource.PlayOneShot(shotsound);


    }


    public void StartControl()
    {
        position = 0.5f;
    }

    public event Action<IControlled> OnControlEnd;

    public void EndControl()
    {
        //cleanup here.
        position = 0.5f;
    }

    private void EndInteraction()
    {
    
        OnControlEnd?.Invoke(this);
    }

    public void Move(Vector2 move)
    {
        position += move.y * .5f;
        position= Mathf.Clamp(position, 0, 1);
        TurrentCart.transform.position = turrentCartStartPos+Vector3.Lerp(begin.position, end.position, position);
    }

    public void Rotate(Vector2 rotate)
    {

       var percentage=  (rotate.y + 1) / 2;
       Turret.rotation = Quaternion.Euler(Vector3.Lerp(AimDirMin, AimDirMax, percentage));

    }

    public void OnTrigger()
    {
        Shoot();
    }

    public void Yes()
    {
    
    }

    public void No()
    {
      
    }

    public void Interact()
    {
      EndInteraction();
    }
    
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(begin.position, .5f);
        
        Gizmos.DrawSphere(end.position, .5f);
    }
}
