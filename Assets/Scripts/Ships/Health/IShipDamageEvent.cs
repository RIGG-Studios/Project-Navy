public enum ShipDamageEvents
{
    BlowOffMasts,
    None
}

public interface IShipDamageEvent
{
    ShipDamageEvents eventType { get; }

    void Init(Ship ship);
    void InvokeEvent();
}
