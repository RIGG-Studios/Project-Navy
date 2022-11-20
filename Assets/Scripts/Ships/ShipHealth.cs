using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth;
    
    public float Health { get; private set; }

    
    [HideInInspector]
    public UnityEvent onDamageTaken; 

    private void Awake()
    {
        Health = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        Health -= damageAmount;
    }
}
