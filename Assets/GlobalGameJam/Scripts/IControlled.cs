using System;

public interface IControlled
{
    Player ControlledBy { get; set; }
    void StartControl(Player controlledby);
    event Action<IControlled> OnControlEnd;
    void EndControl();
}