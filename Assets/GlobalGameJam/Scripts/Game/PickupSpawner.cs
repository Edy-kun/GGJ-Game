
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickupSpawner : MonoBehaviour
{
  public PickUp PickUpPrototype;
  public List<PickUpProfile> PicList;
  private List<PickUp> spawnedPickups = new List<PickUp>();

  private void Awake()
  {
  }

  private void Start()
  {

      for (var i = 0; i < 30; i++)
      {
          SpawnPickup();
      }
  }

  private void SpawnPickup()
  {
      var pic = Instantiate(PickUpPrototype,new Vector3(Random.value*1000,0,Random.value*1000)-new Vector3(500,0,500),Quaternion.identity);
      spawnedPickups.Add(pic);
      pic.SetProfile(PicList[Random.Range(0,PicList.Count)]);
      pic.OnPickedUp += Unsub;
      
  }
  

  private void Unsub(PickUp p)
  {
      SpawnPickup();
      spawnedPickups.Remove(p);
  }
}