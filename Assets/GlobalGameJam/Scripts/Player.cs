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
        this.transform.Rotate(Vector3.up, Vector2.Angle(Vector2.zero, rotation));
    }

    public void Move(Vector2 movement)
    {
       this.transform.Translate(movement);
    }

    public void Interact()
    {
        if (Physics.Raycast(this.transform.position, this.transform.forward, out var hit))
        {
            var con = hit.rigidbody.GetComponent<IControlled>();
            if (con == null) 
                return;
            con.StartControl();
            con.OnControlEnd += EndControl;
            

        }
        
    }

    private void EndControl()
    {
        _controller = null;
    }

    public void EndInteraction()
    {
        throw new NotImplementedException();
    }
}

public interface IControlled
{
    void StartControl();
    event Action OnControlEnd;
    void Rotate(Vector2 rotation);
    void Move(Vector2 movement);
    void Interact();
    void EndInteraction();
    
}