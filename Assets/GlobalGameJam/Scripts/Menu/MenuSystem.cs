﻿using UnityEngine;

public class MenuSystem : MonoBehaviour
{
    public UIPanel 
        MenuPanel,
        Players,
        Credits;

    private UIPanel activePanel;
    private void Awake()
    {
        Show(MenuPanel);
        Credits.MenuSystem = Players.MenuSystem = MenuPanel.MenuSystem = this;
    }

    public void Show(UIPanel menuSystemPlayers)
    {
        activePanel?.Hide();
        activePanel = menuSystemPlayers;
        activePanel.Show();
    }

    public void HandleQuit()
    {
        Application.Quit();
    }
}