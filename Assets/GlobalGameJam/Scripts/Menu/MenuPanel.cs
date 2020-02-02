using System;
using System.Collections;
using System.Collections.Generic;


public class MenuPanel : UIPanel{

  public void HandleStart()
  {
      MenuSystem.Show(MenuSystem.Players);
  }

  public void HandleCredits()
  {
      MenuSystem.Show(MenuSystem.Credits);
  }

  public void HandleQuit()
  {
      MenuSystem.HandleQuit();
  }
  
}