﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Inject]
    private Settings _settings;
    
    public override void InstallBindings()
    {
        InstallHoverCraft();
        InstallEnemy();
    }

    public void InstallHoverCraft()
    {
       

    }

    public void InstallEnemy()
    {
        Container.BindInterfacesAndSelfTo<RandomEnemyPlacement>().AsSingle();
        Container.BindFactory<TurretAI, TurretAI.Factory>()
            .FromComponentInNewPrefab(_settings.Enemy)
            // We can also tell Zenject what to name the new gameobject here
            .WithGameObjectName("Enemy")
            // GameObjectGroup's are just game objects used for organization
            // This is nice so that it doesn't clutter up our scene hierarchy
            .UnderTransformGroup("Enemies");

        Container.BindMemoryPool<BulletCollision, BulletCollision.Pool>()
            .WithInitialSize(10)
            .FromComponentInNewPrefab(_settings.bullet)
            .WithGameObjectName("bullet")
            .UnderTransformGroup("bullets")
            .AsSingle();
        
    }
    
    [Serializable]
    public class Settings
    {
        public int MaxPlayer;
        public GameObject Enemy;
        public GameObject PlayerPrefab;
        public GameObject Hovercraft;
        public GameObject bullet;
    }
}
