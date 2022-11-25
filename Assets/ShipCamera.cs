using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCamera : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 3.0f;
    [SerializeField] private Vector3 offset;

    private Transform _target;

    
    public void Init(Transform target)
    {
        _target = target;
    }

    public void Enable()
    {
 //       transform.SetParent(null);
    }

    public void Disable()
    {
    //    transform.SetParent(_target);
    }

    private void Update()
    {
  //      Vector3 target = _target.position + offset;

     //   transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * lerpSpeed);
       // transform.rotation = Quaternion.Lerp(transform.rotation, _target.rotation, Time.deltaTime * lerpSpeed);
    }
}
