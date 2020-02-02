using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boat : MonoBehaviour, ICanPickUp
{
    
    public List<IRepairable> Devices;
    public Inventory Inventory = new Inventory();
    public RandomEnemyPlacement _spawner;


    private void Awake()
    {
        Inventory.AddElement(new Element(){Amount = 100,type = ElementType.Ammo});
    }

    public void TakeDamage(int dmg)
    {
        Devices[Random.Range(0, Devices.Count)].TakeDamage(dmg);
    }

    public bool TryPickUp(PickUpProfile contains)
    {
        if(contains)
            Inventory.AddElement(contains.Element);
        return true;
    }
}

