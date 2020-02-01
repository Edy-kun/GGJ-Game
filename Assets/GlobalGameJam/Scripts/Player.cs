using UnityEngine;

public class Player : MonoBehaviour, ICanPickUp
{
    public Element holds;
   
    private Vector3 _aimDirection;
    private ControllerStrategy _controller;
    
    
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
}

public abstract class ControllerStrategy
{
    public abstract void Rotate(Vector2 rotation);
    public abstract void Move(Vector2 movement);
    public abstract void Interact();
    public abstract void EndInteraction();
}