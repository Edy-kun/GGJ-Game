﻿using UnityEngine;

[CreateAssetMenu(fileName = "DeviceConfig", menuName = "Custom/DeviceConfig")]
public class DeviceConfig : ScriptableObject
{
    public int health;
    public Element RequiredElemnt;
    
    public ParticleSystem
        BrokenParticle,
        RepairParticle;

    public AudioClip
        BreakSound,
        RepairSound;

}