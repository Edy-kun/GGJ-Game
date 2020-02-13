using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalGameJam.Hovercraft;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator), typeof(Collider))]
public class Player : MonoBehaviour, ICanPickUp, IReceiveInput
{
    public GameObject character;
    public PickUpProfile holds;
    private Vector3 _aimDirection;
    private IControlled _controller;
    private IReceiveInput _receiveInput;
    private bool _isRepairing;
    private IRepairable _repa;
    private static readonly int Repair1 = Animator.StringToHash("Repair");
    private static readonly int Speed = Animator.StringToHash("Speed");

    public Boat Boat { get; set; }
    public bool OnBoat => true; // transform.parent == Boat.transform;
    public Transform boxSnap;
    public Transform toolSnap;
    public Animator anim;
    private Collider _col;
    private Camera _mainCamera;

    public bool TryPickUp(PickUpProfile contains)
    {
        if (holds != null) return false;
        holds = contains;
        return true;
    }

    public float movespeed = 10;
    private Vector3 _movevec;
    private bool _triggerActive;

    public void Update()
    {
        if (_isRepairing) return;
        if (OnBoat)
        {
            var x = transform.localPosition.x;
            var z = transform.localPosition.z;
            if (x > 3 || x < -3 || z < -4 || z > 3)
            {
                /*
                _rb = gameObject.AddComponent<Rigidbody>();
                _rb.isKinematic = true;
                */
                //Debug.Log("Disembark");
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                transform.parent = null;
                _col.isTrigger = false;
            }
        }
        else
        {
            var inv = Boat.transform.InverseTransformPoint(transform.position);
            if (inv.x < 3 && inv.x > -3 && inv.z > -4 && inv.z < 3)
            {
                Destroy(_rb);
                transform.parent = Boat.transform;
                transform.localPosition = new Vector3(inv.x, 1.6f, inv.z);
                if (Boat.TryPickUp(holds)) holds = null;
                _col.isTrigger = true;
            }
        }


        // DoMove(_movevec * Time.deltaTime);
        if (_triggerActive)
        {
            _receiveInput?.OnTriggerLeft(1f);
        }
    }

    private Rigidbody _rb;

    private void Awake()
    {
        _col = GetComponent<Collider>();
        _mainCamera = Camera.main;
    }

    private void Start(){

        /* debug guncontroll
    _receiveInput = FindObjectOfType<Gun>().GetComponent<IReceiveInput>();
    _controller = _receiveInput;
    _controller.StartControl(this);
    _receiveInput.OnControlEnd += EndControl;
    */
        
    }
    public void DoMove(Vector3 vec)
    {
        if (IsRepairing)
            vec = Vector3.zero;
        if (_receiveInput == null && _controller == null)
        {
            Vector3 move;
            if (OnBoat)
            {
                move = new Vector3(vec.x, 0, vec.y) * movespeed;

                if (move.magnitude != 0)
                {
                    var rotation = Quaternion.LookRotation(Boat.transform.TransformDirection(move));
                    transform.rotation = rotation;
                    transform.localPosition += move;
                }
            }
            else
            {
                var camRight = _mainCamera.transform.right;
                var forward = Vector3.Cross(camRight, Vector3.up);
                var toCamera = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(forward, Vector3.up), Vector3.one);
                var worldMove = toCamera.MultiplyVector(new Vector3(vec.x, 0, vec.y)) * movespeed;
                //  Debug.Log(string.Join(",", new[] {worldMove.x, worldMove.y, worldMove.z}));
                transform.LookAt(transform.position + worldMove);

                transform.position += worldMove;
                move = worldMove;
            }

            anim.SetFloat(Speed, move.magnitude);
        }
        else
        {
            _receiveInput?.StickLeft(new Vector2(vec.x,vec.z));
        }
    }

    /*
    public void Move(InputAction.CallbackContext context)
    {
        if (!IsMine(context) || IsRepairing)
            return;

        /*if (_controller is ThirdPersonHoverCraftController hoverCraftController)
        {
            hoverCraftController.ControlLeftThruster(context);
            return;
        }#1#

        _movevec = context.ReadValue<Vector2>();
    }
    */

    
    private bool IsMine(InputAction.CallbackContext context)
    {
        //return context.control.device == InputDevice;
        return true;
    }

    /*
    public void Rotate(InputAction.CallbackContext context)
    {
        if (!IsMine(context) || !OnBoat || IsRepairing)
            return;

        /*
        if (_controller is ThirdPersonHoverCraftController hoverCraftController)
        {
            hoverCraftController.ControlRightThruster(context);
            return;
        }
        #1#

        var rotvec = context.ReadValue<Vector2>();
        _receiveInput?.StickRight(rotvec);
    }
    */

    /*
    public static float Angle(Vector2 p_vector2)
    {
        if (p_vector2.x < 0)
            return 360 - Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1;
        return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
    }*/

    public void Yes()
    {

        if (OnBoat)
        {

            var vols = Physics.OverlapSphere(transform.position, 1f);
            var objects = FindObjectsOfType<Device>();
            IRepairable repaierable = objects.Where(item => item is IRepairable)
                .FirstOrDefault(item => item.NeedsRepair());


            if (repaierable != null)
            {
                Debug.Log("IREPAIING");
                if (repaierable.NeedsRepair())
                    if (Boat.Inventory.TrySubstract(repaierable.GetRequiredItem()))
                    {
                        _repa = repaierable;
                        transform.LookAt((repaierable as MonoBehaviour).transform);
                        DoRepair();
                    }

                return;
            }

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit, 1f))
            {
                _receiveInput = hit.transform.GetComponent<IReceiveInput>();
                if (_receiveInput != null)
                {
                    _movevec = Vector2.zero;
                    _controller = _receiveInput;
                    _receiveInput.OnControlEnd += EndControl;
                }
            }
        }
    }

    /*
    public void No(InputAction.CallbackContext context)
    {
        if (!IsMine(context) || !OnBoat || IsRepairing)
            return;
        
        _receiveInput?.HandleStopInteract();
    }*/


    /*public void Join(InputAction.CallbackContext context)
    {
        if (InputDevice == null && !_knownControllers.Contains(context.control.device) &&
            context.phase == InputActionPhase.Started)
        {
            transform.parent = Boat.transform;
            var device = context.control.device;
            _knownControllers.Add(device);
            InputDevice = device;
            character.SetActive(true);
            OnJoined?.Invoke(this, this);
        }
    }*/

    /*
    public void Leave(InputAction.CallbackContext context)
    {
        if (InputDevice != null && context.phase == InputActionPhase.Performed)
        {
            var device = context.control.device;
            _knownControllers.Remove(device);
            InputDevice = null;
            OnLeave?.Invoke(this, this);
            character.SetActive(false);
        }
    }*/

    public void LeftTrigger(InputAction.CallbackContext context)
    {
        if (!IsMine(context) || !OnBoat || IsRepairing)
            return;
        
        /*
        if (_controller is ThirdPersonHoverCraftController hoverCraftController)
            hoverCraftController.ControlThrustUpLeft(context);
            */
    }

    public bool IsRepairing = false;

    public void RightTrigger(InputAction.CallbackContext context)
    {
        if (!IsMine(context) || !OnBoat || IsRepairing)
            return;
        
        /*
        if (_receiveInput != null)
            TriggerActive = context.ReadValue<float>() > .1f;
            */
      

        /*
        if (_controller is ThirdPersonHoverCraftController hoverCraftController)
            hoverCraftController.ControlThrustUpRight(context);*/
    }
    

    public void StartControl()
    {
    }

    public void DoRepair()
    {
        Repair(4.5f);
    }

    public IEnumerable Repair(float time)
    {
        anim.SetBool(Repair1, true);
        _isRepairing = true;
        yield return new WaitForSeconds(time);
        _isRepairing = false;
        anim.SetBool(Repair1, false);
        _repa.Repair();
    }

    public void EndControl()
    {
        //do cleanup here
        _controller = null;
        _receiveInput = null;
    }

    public Player ControlledBy { get; set; }

    public void StartControl(Player controlledby)
    {
        throw new NotImplementedException();
    }

    public event Action<IControlled> OnControlEnd;
    /*
    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed || !IsMine(context) || !OnBoat || IsRepairing)
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


        if (Physics.Raycast(transform.position, transform.forward, out var hit, 5.0f))
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

            if (icon == null && icon.ControlledBy != null)
                return;

            icon.OnControlEnd += EndControl;
            icon.StartControl(this);
            _controller = icon;
        }
    }*/

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

    public void StickLeft(Vector2 vec)
    {
        
        if(_receiveInput != null)
            _receiveInput.StickLeft(vec);
        else
        {
            _movevec = vec;
        }
    }

    public void StickRight(Vector2 vec)
    {
        _receiveInput?.StickRight(vec);

    }

    public void OnTriggerLeft(float f)
    {
        _receiveInput?.OnTriggerLeft(f);
    }

    public void OnTriggerRight(float f)
    {
        _receiveInput?.OnTriggerRight(f);
    }

    public void HandleInteract()
    {
        if (_receiveInput != null)
        {
            _receiveInput.HandleInteract();
        }
        else
        {
           Yes();
        }
    }

    public void HandleStopInteract()
    {
        _receiveInput?.HandleStopInteract();
    }

    public void HandleRepair()
    {
        throw new NotImplementedException();
    }
}