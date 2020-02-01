using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Device:MonoBehaviour,IRepairable
{
    protected AudioSource audioSource;
    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    public DeviceConfig config;
    
    public int Health { get; set; }
    public void TakeDamage(int dmg)
    {
        Health -= dmg;
        if (Health < 0)
        {
            
        }
    }

    public void SetConfig(DeviceConfig _config)
    {
        config = _config;
        Health = config.health;
        

    }

    public List<Element> GetRequiredItem()
    {
        throw new System.NotImplementedException();
    }

    public void Repair()
    {
        Health = config.health;
        audioSource.PlayOneShot(config.RepairSound);
    }

    public void Break()
    {
        audioSource.PlayOneShot(config.BreakSound);
    }
}