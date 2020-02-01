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

    public void StartControl()
    {
        throw new NotImplementedException();
    }

    public void EndControl()
    {
        //do cleanup here
    }
    public event Action<IControlled> OnControlEnd;
    
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

    private void EndControl(IControlled controlled)
    {
        Debug.Assert(_controller == controlled);
        _controller = null;
        controlled.OnControlEnd -= EndControl;
        controlled.EndControl();
    }

}

public interface IControlled
{
    void StartControl();
    event Action<IControlled> OnControlEnd;
    void EndControl();


}