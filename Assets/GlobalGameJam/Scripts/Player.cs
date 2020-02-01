using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor;
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
    public event Action<object, Player> OnLeave;
    private static HashSet<InputDevice> _knownControllers= new HashSet<InputDevice>();


    private void Awake()
    {
        character.SetActive(false);
    }


    public bool TryPickUp(Element contains)
    {
        if (holds != null)
            return false;

        holds = contains;
        return true;
    }


    public float movespeed = 5;
    private Vector3 _movevec;

    public void Update()
    {
        DoMove(_movevec*Time.deltaTime);
    }

    public void DoMove(Vector3 vec)
    {
        if (_recieveInput == null)
        {
            this.transform.position += vec * (movespeed);
        }
        else
        {
            _recieveInput.Move(vec);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!IsMine(context)) 
            return;
        var temp = context.ReadValue<Vector2>();
        _movevec = new Vector3(temp.x, 0, temp.y);


    }

    private bool IsMine(InputAction.CallbackContext context)
    {
        return (context.control.device == InputDevice);
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        if (!IsMine(context)) return;
       
            var rotvec = context.action.ReadValue<Vector2>();
            this.transform.rotation = Quaternion.Euler(0, Angle(rotvec), 0);
        
    }

    private static float Angle(Vector2 p_vector2)
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
        if (!IsMine(context)) return;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit, 1f))
        {
            _recieveInput = hit.rigidbody.GetComponent<IReceiveInput>();
        }


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

    public event Action<object, Player> OnJoined;
}

public interface IReceiveInput
{
    void Move(Vector3 move);
    void Rotate(Vector3 rotate);

    void Yes();
    void No();
    void Interact();
}
