using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShipController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float movementSpeed;

    private Rigidbody _rigidbody;
    
    private void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
    }


    private void FixedUpdate()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        
        _rigidbody.AddForce(dir * movementSpeed, ForceMode.Acceleration);
    }
}
