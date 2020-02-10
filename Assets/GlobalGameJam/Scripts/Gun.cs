using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

[RequireComponent(typeof(BoxCollider))]
public class Gun : Device, IControlled, IReceiveInput
{
    public float fireRate
    {
        get => 0.05f;
        set { fireRate = value; }
    }

    public float _previousFire;
    private Element Cost = new Element() { Amount =  1, type = ElementType.Ammo};

    private float position =.5f;
    public Transform begin;
    public Transform end;
    public bool flip;

    public Vector3 AimDirMax;
    public Vector3 AimDirMin;
    public Transform TurrentCart;
    public Transform Turret;
    private Vector3 turrentCartStartPos;
    public Vector3 shotDir;
    public AudioClip shotsound;
    public GameObject bullet;
    private RandomEnemyPlacement _spawner;
    private BulletCollision.Pool _pool;
    [Inject]
    public void construct(RandomEnemyPlacement spawner, BulletCollision.Pool pool)
    {
        _spawner = spawner;
        _pool = pool;
    }
    
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
            if (!Boat.Inventory.TrySubstract(Cost)) 
                return;
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

        var shootBullet = _pool.Spawn();
        bullet.transform.position = Turret.position + bulletOffset; //, Turret.rotation);
        bullet.transform.LookAt(Turret.TransformDirection(shotDir));
        shootBullet.GetComponent<Rigidbody>().AddForce(Turret.TransformDirection(shotDir) * 90f);
      //  Destroy(shootBullet, 0.4f);
        audioSource.PlayOneShot(shotsound);

        
        var hitable = _spawner.CheckHit(Turret.position, Turret.right).FirstOrDefault();
        if (hitable)
        {
            Debug.Log("HIT");
            hitable.TakeDamage(50);
        }



    }




    public void StartControl(Player controlledBy)
    {
        ControlledBy = controlledBy;
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

    public void StickLeft(Vector2 move)
    {
        position += move.y * .5f;
        position= Mathf.Clamp(position, 0, 1);
        
        var vecOffset = Vector3.Lerp( end.position, begin.position,position);
        var playerOffset = TurrentCart.transform.localPosition;
        TurrentCart.transform.position = turrentCartStartPos + vecOffset;
        playerOffset -= TurrentCart.transform.localPosition;
        ControlledBy.transform.localPosition -= new Vector3(playerOffset.x, 0, playerOffset.z);
    }

    public void StickRight(Vector2 rotate)
    {

        var percentage = (rotate.y + 1) / 2;
        
        Turret.localRotation = Quaternion.Euler(Vector3.Lerp(AimDirMin, AimDirMax, percentage)+ new Vector3(0,flip?180:0,0));

        var hitable = GameManager.Instance._spawner.CheckHit(Turret.position, Turret.right).FirstOrDefault();
        if (hitable)
        {
            hitable.TakeDamage(25);
        }
        


    }

    public void OnTriggerLeft(float f)
    {
       // Shoot();
    }

    public void OnTriggerRight(float f)
    {
        Shoot();
    }

    public void HandleInteract()
    {
        
    }

    public void HandleStopInteract()
    {
        EndInteraction();
    }

    public void HandleRepair()
    {
      //EndInteraction();
    }
    
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(begin.position, .5f);
        Gizmos.DrawRay(Turret.position,  Turret.right*1000f);
        Gizmos.DrawSphere(end.position, .5f);
    }
}
