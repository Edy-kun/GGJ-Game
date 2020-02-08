using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerV2 : MonoBehaviour
{
    public IReceiveInput InputReciever;
    public Player p;

    private void Awake()
    {
       // throw new NotImplementedException();
       InputReciever = FindObjectOfType<Player>();
    }

    public void OnLeftStick(InputValue obj)
    {
        var move = obj.Get<Vector2>();
      //  this.transform.position += new Vector3(move.x,0,move.y);
        InputReciever.StickLeft(move);
        
    }
    
    public void OnRightStick(InputValue obj)
    {
        var move = obj.Get<Vector2>();
      //  this.transform.position += new Vector3(move.x,0,move.y);
      InputReciever.StickRight(move);

    }
    
    public void OnLeftTrigger(InputValue obj)
    {
        var move = obj.Get<float>();
        Debug.Log($"VALUE = {move}" );
        InputReciever.OnTriggerLeft(move);   
   
    }
    
    public void OnRightTrigger(InputValue obj)
    {
        var move = obj.Get<float>();
        InputReciever.OnTriggerRight(move);   

    }

    public void OnInteract(InputValue obj)
    {
        InputReciever.HandleInteract();

    }
    public void OnCancel(InputValue obj)
    {
      InputReciever.HandleStopInteract();
    }
}