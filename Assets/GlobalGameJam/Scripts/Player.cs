﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalGameJam.Hovercraft;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;



[RequireComponent(typeof(Animator),typeof(Collider))]
public class Player : MonoBehaviour, ICanPickUp//, IControlled
{
    public GameObject character;
    public PickUpProfile holds;
    private Vector3 _aimDirection;
    private IControlled _controller;
    private IReceiveInput _receiveInput = null;
    public InputDevice InputDevice;
    [SerializeField] private PlayerInput _playerInput;
    public event Action<object, Player> OnLeave;
    private static HashSet<InputDevice> _knownControllers = new HashSet<InputDevice>();
    public Boat Boat { get; set; }

    public bool Onboat => transform.parent == Boat.transform;
    
    public Transform BoxSnap;
    public Transform ToolSnap;
    public Animator Anim;
    private Collider col;
    private Camera mainCamera = null;

    public bool TryPickUp(PickUpProfile contains)
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
        if (IsRepairing)
            return;

        if (Onboat)
        {
            var x = this.transform.localPosition.x;
            var z = this.transform.localPosition.z;
            if (x > 3 || x < -3 || z < -4 || z > 3)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                //Debug.Log("Disembark");
                this.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);
                this.transform.parent = null;
                col.isTrigger = false;
            }
        }
        else
        {
            var inv = Boat.transform.InverseTransformPoint(this.transform.position);

            if (inv.x < 3 && inv.x > -3 && inv.z > -4 && inv.z < 3)
            {
                Destroy(rb);
                this.transform.parent = Boat.transform;
                this.transform.localPosition = new Vector3(inv.x, 1.6f, inv.z);
                if (Boat.TryPickUp(holds))
                    holds = null;
                col.isTrigger = true;

            }
        }



        DoMove(_movevec * Time.deltaTime);
        if (TriggerActive)
        {
            if (_receiveInput != null)
            {
                _receiveInput.OnTrigger();
            }
        }



    }

    private Rigidbody rb;
    private void Awake()
    {
        col = this.GetComponent<Collider>();
        mainCamera = Camera.main;



    }

    public void DoMove(Vector3 vec)
    {
        if (IsRepairing)
            vec = Vector3.zero;
        if (_receiveInput == null && _controller==null)
        {
            Vector3 move;
            if (Onboat)
            {  move = new Vector3(vec.x, 0, vec.y) * movespeed;

                if (move.magnitude != 0)
                {
                    var rotation = Quaternion.LookRotation(Boat.transform.TransformDirection(move));
                    transform.rotation = rotation;
                    transform.localPosition += move;
                }
            }
            else
            {

               
                
                var camRight = mainCamera.transform.right;
                var forward = Vector3.Cross(camRight, Vector3.up);
                var toCamera = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(forward, Vector3.up), Vector3.one);
                var worldMove = toCamera.MultiplyVector(new Vector3(vec.x, 0, vec.y)) * (movespeed);
              //  Debug.Log(string.Join(",", new[] {worldMove.x, worldMove.y, worldMove.z}));
                transform.LookAt(this.transform.position + worldMove);

                transform.position += worldMove;
                move = worldMove;
            }

            Anim.SetFloat("Speed", move.magnitude);
            
        }
        else
        {
            _receiveInput?.Move(vec);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!IsMine(context)|| IsRepairing)
            return;

        if (_controller is ThirdPersonHoverCraftController hoverCraftController)
        {
            hoverCraftController.ControlLeftThruster(context);
            return;
        }

        _movevec = context.ReadValue<Vector2>();
   
    }

    private bool IsMine(InputAction.CallbackContext context)
    {
        return (context.control.device == InputDevice);
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        if (!IsMine(context)||!Onboat|| IsRepairing)
            return;
        
        if (_controller is ThirdPersonHoverCraftController hoverCraftController)
        {
            hoverCraftController.ControlRightThruster(context);
            return;
        }

        var rotvec = context.ReadValue<Vector2>();
        if (_receiveInput != null)
        {
            _receiveInput.Rotate(rotvec);
        }
        else
        {
           // this.transform.rotation = Quaternion.Euler(0, Angle(rotvec), 0);
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
        if (!IsMine(context)|| IsRepairing)
            return;
        if (Onboat)
        {
            if (_receiveInput == null)
            {

                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit, 1f))
                {
                    var repaierable = hit.transform.GetComponent<IRepairable>();
                    
                    if (repaierable != null)
                    {
                        Debug.Log("IREPAIING");
                        if (repaierable.NeedsRepair())
                        {
                          
                            if (Boat.Inventory.TrySubstract(repaierable.GetRequiredItem()))
                            {repa = repaierable;
                                transform.LookAt((repaierable as MonoBehaviour).transform);
                                DoRepair();  
                            }

                        }
                        
                        return;
                    }
                    _receiveInput = hit.transform.GetComponent<IReceiveInput>();
                    if (_receiveInput != null)
                    {
                        _movevec = Vector2.zero;
                        _controller = _receiveInput;
                        _receiveInput.OnControlEnd += EndControl;
                    }
                  
                }
            }
            else
            {
                _receiveInput.Yes();
            }
        }
     
    }

    public void No(InputAction.CallbackContext context)
    {
        if (!IsMine(context)||!Onboat|| IsRepairing)
            return;
        if (_receiveInput != null)
        {
            _receiveInput.No();
        }
      
    }


    public void Join(InputAction.CallbackContext context)
    {
        if (InputDevice == null && !_knownControllers.Contains(context.control.device) &&
            (context.phase == InputActionPhase.Started))
        {
            this.transform.parent = Boat.transform;
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
        if (!IsMine(context)||!Onboat|| IsRepairing)
            return;
        if (_controller is ThirdPersonHoverCraftController hoverCraftController)
        {
            hoverCraftController.ControlThrustUpLeft(context);
        }
    }

    public void RightTrigger(InputAction.CallbackContext context)
    {
        if (!IsMine(context)||!Onboat|| IsRepairing)
            return;
        if (_receiveInput != null)
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

    private bool IsRepairing = false;
    private IRepairable repa;
    private static readonly int Repair1 = Animator.StringToHash("Repair");

    public void DoRepair()
    {
        Repair(4.5f);
    }

    public IEnumerable Repair(float time)
    {  
        Anim.SetBool(Repair1, true);
        IsRepairing = true;
        yield return new WaitForSeconds(time);
        IsRepairing = false;
        Anim.SetBool(Repair1, false);
        repa.Repair();
    }
    

    public void EndControl()
    {
        //do cleanup here
        _controller = null;
        _receiveInput = null;
    }

    public event Action<IControlled> OnControlEnd;

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsMine(context) || !Onboat || IsRepairing)
            return;


        if (_controller is ThirdPersonHoverCraftController hoverCraftController)
        {
            hoverCraftController.RequestEndControl(context);
            return;
        }

        if (_receiveInput != null)
        {
            _receiveInput.Interact();
            return;
        }


        if (Physics.Raycast(this.transform.position, this.transform.forward, out var hit, 5.0f))
        {
            var con = hit.collider.GetComponent<IReceiveInput>();
            IControlled icon = null;
            if (con != null)
            {
                icon = _receiveInput = con;

            }
            else
            {
                Debug.Log($"{name} tried to interact with {hit.rigidbody.name}");
                icon = hit.rigidbody.GetComponent<IControlled>();
            }

            if (icon == null && icon.ControlledBy!=null)
                return;

            icon.OnControlEnd += EndControl;
            icon.StartControl(this);
            _controller = icon;
        }
    }

    private void EndControl(IControlled controlled)
    {
        Debug.Assert(_controller == controlled);
        _controller = null;
        _receiveInput = null;
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