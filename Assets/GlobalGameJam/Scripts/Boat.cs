using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boat : MonoBehaviour, ICanPickUp
{
    
    public List<IRepairable> Devices;
    public Inventory Inventory = new Inventory();
    public RandomEnemyPlacement _spawner;

    /*
    public Transform DisembarkPosition;
    public Transform EmbarkPosition;
    

    public Vector3 GetDisembarkLocation()
    {
        return new Vector3(DisembarkPosition.transform.position.x,0,DisembarkPosition.position.z);
    }
    public Vector3 GetEmbarkLocation()
    {
        return EmbarkPosition.position;
    }
   */
    public void TakeDamage(int dmg)
    {
        Devices[Random.Range(0, Devices.Count)].TakeDamage(dmg);
    }

    public bool TryPickUp(PickUpProfile contains)
    {
        if(contains)
            Inventory.items.Add(contains.Element);
        return true;
    }
}

