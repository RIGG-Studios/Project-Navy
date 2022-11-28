using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipVitalPoint : MonoBehaviour, IDamagable
{
    [SerializeField] private ShipHealth shipHealth;
    
    public int ActorID { get; set; }

    public ShipHealth ShipHealth => shipHealth;

    private void Start()
    {
        if (shipHealth == null)
        {
            return;
        }
        
        ActorID = shipHealth.Ship.OwnerActorNumber;
    }

    public void Damage(int attackerID, float damageAmount)
    {
        if (shipHealth == null)
        {
            return;
        }
        
        shipHealth.Damage(damageAmount);
    }
}
