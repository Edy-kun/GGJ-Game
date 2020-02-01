using System;
using System.Collections.Generic;
using System.Linq;
using GlobalGameJam.Hovercraft;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour, ICanPickUp, IControlled
{
    public GameObject character;
    public Element holds;
    private Vector3 _aimDirection;
    private IControlled _controller;
    private IReceiveInput _recieveInput = null;
    public InputDevice InputDevice;
    [SerializeField] private PlayerInput _playerInput;
    public event Action<object, Player> OnLeave;
    private static HashSet<InputDevice> _knownControllers= new HashSet<InputDevice>();
    
    public bool TryPickUp(Element contains)
    {
        if (holds != null)
            return false;

        holds = contains;
        return true;
    }


    public float movespeed = 10;
    private Vector3 _movevec;

    private bool TriggerActive;
    public void Update()
    {
        DoMove(_movevec*Time.deltaTime);
        if (TriggerActive)
        {
            if (_recieveInput != null)
            {
                _recieveInput.OnTrigger();
            }
        }
            
    }

    public void DoMove(Vector3 vec)
    {
        if (_recieveInput == null)
        {
            var move = new Vector3(vec.x,0,vec.y);
            transform.position += move * movespeed;
        }
        else
        {
            _recieveInput.Move(vec);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
    if(_controller is ThirdPersonHoverCraftController hoverCraftController)
     hoverCraftController.ControlLeftThruster(context);
        if (!IsMine(context)) 
            return;
        _movevec = context.ReadValue<Vector2>();
   


    }

    private bool IsMine(InputAction.CallbackContext context)
    {
        return (context.control.device == InputDevice);
    }

    public void Rotate(InputAction.CallbackContext context)
    { 
        if (!IsMine(context))
                 return;
        if (_controller is ThirdPersonHoverCraftController hoverCraftController)
        {
            hoverCraftController.ControlRightThruster(context);
            return;
        }

        var rotvec = context.ReadValue<Vector2>();
        if (_recieveInput != null)
        {
            _recieveInput.Rotate(rotvec);
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(0, Angle(rotvec), 0);
        }

    }

    public static float Angle(Vector2 p_vector2)
    {
        if (p_vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }

    public void Yes(InputAction.CallbackContext context)
    {
        if (!IsMine(context)) 
            return;
        if (_recieveInput == null)
        {

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit, 1f))
            {
                _recieveInput = hit.transform.GetComponent<IReceiveInput>();
                _controller = _recieveInput;
                _recieveInput.OnControlEnd += EndControl;

            }
        }
        else
        {
            _recieveInput.Yes();
        }

    }

    public void No(InputAction.CallbackContext context)
    {
        if (_recieveInput != null)
        {
            _recieveInput.No();
        }
        
    }


    public void Join(InputAction.CallbackContext context)
    {

        if (InputDevice == null && !_knownControllers.Contains(context.control.device)&&(context.phase == InputActionPhase.Started))
        {
            var device = context.control.device;
            _knownControllers.Add(device);
            InputDevice = device;
            character.SetActive(true);
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
            character.SetActive(false);
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
        if (!IsMine(context))
            return;
        if (_recieveInput != null)
        {
            TriggerActive =     context.ReadValue<float>() > .1f;
            //_recieveInput.OnTrigger();
        }
        
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
        _recieveInput = null;
    }
    public event Action<IControlled> OnControlEnd;
    
    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsMine(context)) 
            return;
        

        if (_controller is ThirdPersonHoverCraftController hoverCraftController)
        {
            hoverCraftController.RequestEndControl(context);
            return;
        }

        if (_recieveInput != null)
        {
            _recieveInput.Interact();
            return;
        }

        Debug.Log("Interacted");
        if (Physics.Raycast(this.transform.position, this.transform.forward, out var hit, 5.0f))
        {
            
            Debug.Log($"{name} tried to interact with {hit.rigidbody.name}");
            var con = hit.rigidbody.GetComponent<IControlled>();
            if (con == null) 
                return;
            else if (con is IReceiveInput input)
            {
                _recieveInput = input;
            }
            con.OnControlEnd += EndControl;
            con.StartControl();
            _controller = con;
        }
    }

    private void EndControl(IControlled controlled)
    {
        Debug.Assert(_controller == controlled);
        _controller = null;
        _recieveInput = null;
        controlled.OnControlEnd -= EndControl;
        controlled.EndControl();
    }

    public event Action<object, Player> OnJoined;

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.forward);
    }
}

public interface IReceiveInput : IControlled
{
    void Move(Vector2 move);
    void Rotate(Vector2 rotate);

    void OnTrigger();
    void Yes();
    void No();
    void Interact();
}
