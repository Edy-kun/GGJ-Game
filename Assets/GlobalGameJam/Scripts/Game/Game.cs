using System;
using UnityEngine;


public class Game: SceneController
{

    public HUD hud;
    public PickupSpawner PickupSpawner;

    public override void Start()
    {
        base.Start();
        PickupSpawner = new PickupSpawner();
        
    }

    private void Update()
    {
        PickupSpawner.Update();
    }
}

public class PickupSpawner
{
    public void Update()
    {
        
    }
}
