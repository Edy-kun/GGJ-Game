using System;
using UnityEngine;


public class Game: SceneController
{

    public HUD hud;
    //public PickupSpawner PickupSpawner;
    public Team team;

    public override void Start()
    {
        base.Start();
      //  PickupSpawner = new PickupSpawner();
        team.OnScoreChanged += hud.HandleScoreChanged;

    }

    private void Update()
    {
       // PickupSpawner.Update();
    }
}