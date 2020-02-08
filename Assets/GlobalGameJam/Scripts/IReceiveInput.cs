using UnityEngine;

public interface IReceiveInput : IControlled
{
    void StickLeft(Vector2 vec);
    void StickRight(Vector2 vec);
    void OnTriggerLeft(float f);
    void OnTriggerRight(float f);
    void HandleInteract();
    void HandleStopInteract();
    void HandleRepair();
}