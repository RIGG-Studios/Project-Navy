using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipVitalPoint : MonoBehaviour, IDamagable
{
    [SerializeField] private ShipHealth shipHealth;
    
    public int ActorID { get; }
    
    public void Damage(int attackerID, float damageAmount)
    {
        shipHealth.Damage(damageAmount);
    }
}
