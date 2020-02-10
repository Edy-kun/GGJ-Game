using System;
using System.Collections;
using System.Collections.Generic;
using GlobalGameJam.Hovercraft;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoverCraftUI : MonoBehaviour
{
    [SerializeField] private ThirdPersonHoverCraftController _hoverCraftController;

    [SerializeField] private Slider _leftEngineSlider;
    [SerializeField] private Slider _rightEngineSlider;
    [SerializeField] private Slider _upSlider;

    
    [SerializeField] private Slider _leftEngineRealOutput;
    [SerializeField] private Slider _rightEngineRealOutput;
    [SerializeField] private Slider _upSliderRealOutput;

    public TMP_Text
        lbl_ammo,
        lbl_tape,
        lbl_prop,
        lbl_score;

    private void Awake()
    {
       
        
    }

    private void Start()
    {
        GameManager.Instance._boat.Inventory.OnElementsChanged+=OnInventoryChanged;
        GameManager.Instance._team.OnScoreChanged += OnScoreChange;
        OnScoreChange(0);
    }

    public void Update()
    {
        _leftEngineSlider.value = _hoverCraftController.LeftEngine.EnginePower;
        _rightEngineSlider.value = _hoverCraftController.RightEngine.EnginePower;
        _upSlider.value = _hoverCraftController.ThrusterController.PowerSetting;

        _leftEngineRealOutput.value = _hoverCraftController.LeftEngine.Effectiveness *
                                      _hoverCraftController.LeftEngine.EnginePower;
        _rightEngineRealOutput.value = _hoverCraftController.RightEngine.Effectiveness *
                                      _hoverCraftController.RightEngine.EnginePower;
        
    }

    public void OnInventoryChanged(Inventory inv)
         {
             lbl_ammo.text = $"Ammo: {inv.elements[ElementType.Ammo]}";
             lbl_tape.text = $"Tape: {inv.elements[ElementType.Tape]}";
             lbl_prop.text = $"Engines: {inv.elements[ElementType.Prop]}";
             
         }
    public void OnScoreChange(int poins)
    {
        lbl_score.text = $"{poins}";
      
    }
    
}
