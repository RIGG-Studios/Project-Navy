using UnityEngine;


[RequireComponent(typeof(BuoyancyObject))]
public class Ship : MonoBehaviour
{
    public Transform playerSpawnPoint;
    
    public BuoyancyObject ShipBuoyancy { get; private set; }
    
    private void Awake()
    {
        ShipBuoyancy = GetComponent<BuoyancyObject>();
    }
}
