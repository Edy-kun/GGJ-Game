using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boat : MonoBehaviour, ICanPickUp
{
    
    public List<IRepairable> Devices;
    public Inventory Inventory = new Inventory();

    public void EnterBoat()
    {
        
    }
    
    public void TakeDamage(int dmg)
    {
        Devices[Random.Range(0, Devices.Count)].TakeDamage(dmg);
    }


    public bool TryPickUp(Element contains)
    {
        Inventory.items.Add(contains);
        return true;
    }
}

