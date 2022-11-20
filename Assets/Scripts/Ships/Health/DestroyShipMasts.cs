using UnityEngine;

public class DestroyShipMasts : MonoBehaviour, IShipDamageEvent
{
    [SerializeField] private Rigidbody[] masts;
    [SerializeField] private float detachForce;


    public ShipDamageEvents eventType => ShipDamageEvents.BlowOffMasts;

    private Ship _ship;
    
    public void Init(Ship ship)
    {
        _ship = ship;
    }

    public void InvokeEvent()
    {
    }
}
