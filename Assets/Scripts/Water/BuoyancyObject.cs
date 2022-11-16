using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyancyObject : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float deptBeforeSubmerged;
    [SerializeField] private float displacementAmount = 3f;
    [SerializeField] private float waterDrag = 0.99f;
    [SerializeField] private float waterAngularDrag = 0.5f;

    private Rigidbody _rigidbody;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
    //    _rigidbody.AddForceAtPosition(Physics.gravity, transform.position, ForceMode.Acceleration);
        
        float waterHeight = Ocean.Instance.GetWaterHeightAtPosition(transform.position, Time.time);
   //     Debug.Log(waterHeight);
        if (transform.position.y < waterHeight)
        {
            float displacement = Mathf.Clamp01((waterHeight - transform.position.y / deptBeforeSubmerged) * displacementAmount);
            Vector3 force = new Vector3(0.0f, Mathf.Abs(Physics.gravity.y) * displacement, 0.0f);
            _rigidbody.AddForceAtPosition(force * this.force, transform.position, ForceMode.Acceleration);
            _rigidbody.AddForce(-_rigidbody.velocity * (displacement * waterDrag * Time.fixedDeltaTime), ForceMode.VelocityChange);
            _rigidbody.AddTorque(-_rigidbody.angularVelocity * (displacement * waterAngularDrag * Time.fixedDeltaTime), ForceMode.VelocityChange);

        }
    }
}
