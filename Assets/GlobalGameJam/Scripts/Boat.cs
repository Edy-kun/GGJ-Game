using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boat : MonoBehaviour, ICanPickUp
{
    public List<IRepairable> Devices;
    public Inventory Inventory = new Inventory();

    public void TakeDamage(int dmg)
    {
        Devices[Random.Range(0, Devices.Count)].TakeDamage(dmg);
    }

    private void Awake()
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        throw new NotImplementedException();
    }

    public bool TryPickUp(Element contains)
    {
        Inventory.items.Add(contains);
        return true;
    }
}

