using System;
using UnityEngine;

public class Menu: SceneController
{
   
}


public abstract class SceneController: MonoBehaviour
{
    public Action OnStart;
    public virtual void Start()
    {
        OnStart?.Invoke();
    }
}