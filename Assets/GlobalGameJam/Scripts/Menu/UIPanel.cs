using System;
using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    [NonSerialized]
    public MenuSystem MenuSystem;
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

  
}