using System;
using System.Collections.Generic;
using System.Linq;
using GlobalGameJam.Hovercraft;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, ICanPickUp, IControlled
{
    public GameObject character;
    public Element holds;
    private Vector3 _aimDirection;
    private IControlled _controller;
    public InputDevice InputDevice;
    [SerializeField] private PlayerInput _playerInput;
    public event Action<object, Player> OnLeave;
    private static HashSet<InputDevice> _knownControllers= new HashSet<InputDevice>();
    
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


    void HandleControl(IControlled controlled)
    {
        
    }

    public void Move(InputAction.CallbackContext context)
    {

       // var movevec = context.action.ReadValue<Vector3>();
       if(_controller is ThirdPersonHoverCraftController hoverCraftController)
           hoverCraftController.ControlLeftThruster(context);
    }

    public void Rotate(InputAction.CallbackContext context)
    {
       // var rotv  = context.action.ReadValue<Vector3>();
       if (_controller is ThirdPersonHoverCraftController hoverCraftController)
           hoverCraftController.ControlRightThruster(context);
    }

    public void Yes(InputAction.CallbackContext context)
    {
    }

    public void No(InputAction.CallbackContext context)
    {
        
    }


    public void Join(InputAction.CallbackContext context)
    {

        if (InputDevice == null && !_knownControllers.Contains(context.control.device)&&(context.phase == InputActionPhase.Started))
        {
            var device = context.control.device;
            _knownControllers.Add(device);
            Debug.Log(context.ToString());
            Debug.Log("device", this);
            InputDevice = device;
            
            OnJoined?.Invoke(this, this);

        }
    }

    public void Leave(InputAction.CallbackContext context)
    {
        if (InputDevice != null && (context.phase == InputActionPhase.Performed))
        {
            
            var device = context.control.device;
            _knownControllers.Remove(device);
            InputDevice = null;
            OnLeave?.Invoke(this, this);
        }
    }

    public void LeftTrigger(InputAction.CallbackContext context)
    {
        if (_controller is ThirdPersonHoverCraftController hoverCraftController)
        {
            hoverCraftController.ControlThrustUpLeft(context);
        }
    }

    public void RightTrigger(InputAction.CallbackContext context)
    {
        if (_controller is ThirdPersonHoverCraftController hoverCraftController)
        {
            hoverCraftController.ControlThrustUpRight(context);
        }
    }

    public void StartControl()
    {
        
    }

    public void EndControl()
    {
        //do cleanup here
    }
    public event Action<IControlled> OnControlEnd;
    
    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (_controller is ThirdPersonHoverCraftController hoverCraftController)
        {
            hoverCraftController.RequestEndControl(context);
            return;
        }

        Debug.Log("Interacted");
        if (Physics.Raycast(this.transform.position, this.transform.forward, out var hit, 5.0f))
        {
            Debug.Log($"{name} tried to interact with {hit.rigidbody.name}");
            var con = hit.rigidbody.GetComponent<IControlled>();
            if (con == null) 
                return;
            con.OnControlEnd += EndControl;
            con.StartControl();
            _controller = con;
        }
    }

    private void EndControl(IControlled controlled)
    {
        Debug.Assert(_controller == controlled);
        _controller = null;
        controlled.OnControlEnd -= EndControl;
        controlled.EndControl();
    }

    public event Action<object, Player> OnJoined;
}