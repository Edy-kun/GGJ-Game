using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Device : MonoBehaviour, IRepairable
{
    protected Transform effectParent;
    protected AudioSource audioSource;
    protected Boat Boat;
    public Player ControlledBy { get; set; }

    protected virtual void Awake()
    {
        effectParent = transform;
        audioSource = this.GetComponent<AudioSource>();
        Health = config.health;
    }

    private void Start()
    {
  

    Boat = GetComponentInParent<Boat>();
    }

    public DeviceConfig config;
    private ParticleSystem _brokenParticles;
    private ParticleSystem _repairParticles;

    public int Health { get; set; }

    public virtual void TakeDamage(int dmg)
    {
        Debug.Log($"{name} taking {dmg} damage");
        
        if (Health > 0 && Health - dmg <= 0)
        {
            Break();
        }

        Health = Mathf.Clamp(Health - dmg, 0, config.health);
    }

    protected float PercentHealth => (float) Health / config.health;

    public void InitDevice(DeviceConfig _config, Boat boat)
    {
        config = _config;
        Health = config.health;
        Boat = boat;
    }

    public abstract List<Element> GetRequiredItem();

    public virtual void Repair()
    {
        Health = config.health;
        if (config.RepairParticle)
        {
            _repairParticles = Instantiate(config.RepairParticle, effectParent, false);
            _repairParticles.transform.localPosition = Vector3.zero;
            Destroy(_repairParticles, _repairParticles.main.duration);
        }

        if (config.RepairSound) audioSource.PlayOneShot(config.RepairSound);
        if (_brokenParticles) Destroy(_brokenParticles.gameObject);
    }

    public virtual void Break()
    {
        if (config.BreakSound) audioSource.PlayOneShot(config.BreakSound);
        if(config.BrokenParticle!=null)
            _brokenParticles = Instantiate(config.BrokenParticle, effectParent, false);
    }
}