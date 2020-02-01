using System;

public interface IControlled
{
    void StartControl();
    event Action<IControlled> OnControlEnd;
    void EndControl();
}