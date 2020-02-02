using System.Collections;
using System.Collections.Generic;
using GlobalGameJam.Hovercraft;
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
}
