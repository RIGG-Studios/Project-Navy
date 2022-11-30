
public enum InteractTypes
{
    Cannon,
    CannonBallHolder,
    Wheel
}

public interface IInteractable
{
    InteractTypes InteractTypes { get; }
    
    void LookAt();
    void StopLookAt();
    void Interact(PlayerController player);
}
