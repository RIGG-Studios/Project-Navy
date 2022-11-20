using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BuoyancyObject))]
public class Ship : MonoBehaviour
{
    public BuoyancyObject ShipBuoyancy { get; private set; }
    
    private void Awake()
    {
        ShipBuoyancy = GetComponent<BuoyancyObject>();
    }
    

    public void SinkShip()
    {
        ShipBuoyancy.enabled = false;
    }
}
