using System;
using System.Collections;
using System.Collections.Generic;
using GlobalGameJam.Hovercraft;
using UnityEngine;

public class EngineInteraction : Device, IRepairable
{
    private HoverCraftEngine _engine { get; set; }
    public bool isLeftEngine;
    [SerializeField] private Transform engineEffectParent;
    [SerializeField] private AudioSource _engineAudioSource;

    private void Start()
    {
        effectParent = engineEffectParent;
        var controller = gameObject.GetComponentInParent<ThirdPersonHoverCraftController>();
        _engine = isLeftEngine ? controller.LeftEngine : controller.RightEngine;
        audioSource = _engineAudioSource;
    }

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        _engine.Effectiveness = Mathf.Clamp01(0.5f + PercentHealth/2f);
    }

    public override void Repair()
    {
        base.Repair();
        _engine.Effectiveness = 1.0f;
    }

    public void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.KeypadMinus)) TakeDamage(10);
        if (Application.isEditor && Input.GetKeyDown(KeyCode.KeypadPlus)) Repair();

    }

    public override List<Element> GetRequiredItem()
    {
        return new List<Element>{new Element(){Amount = 1, type = ElementType.Prop}};
    }
}
