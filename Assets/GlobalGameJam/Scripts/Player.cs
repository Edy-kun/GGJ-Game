using System;
using UnityEngine;

public class Player : MonoBehaviour, ICanPickUp, IControlled
{
    public Element holds;
   
    private Vector3 _aimDirection;
    private IControlled _controller;
    
    
    void Rotate(Vector3 aimDir)
    {
        _aimDirection += aimDir;
    }

    public bool TryPickUp(Element contains)
    {
        if (holds != null)
            return false;

        holds = contains;
        return true;
    }

    private void Update()
    {
        if (_controller != null)
        {
            HandleControl(_controller);
            
        }
        else
        {
            HandleControl(this);
        }
        
    }

    void HandleControl(IControlled controlled)
    {
        
    }

    public void Rotate(Vector2 rotation)
    {
        throw new NotImplementedException();
    }

    public void Move(Vector2 movement)
    {
        throw new NotImplementedException();
    }

    public void Interact()
    {
        throw new NotImplementedException();
    }

    public void EndInteraction()
    {
        throw new NotImplementedException();
    }
}

public interface IControlled
{
    void Rotate(Vector2 rotation);
    void Move(Vector2 movement);
    void Interact();
    void EndInteraction();
}