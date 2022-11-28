using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCamera : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 3.0f;

    private Transform _target;
    private bool _rotate;

    public void Init(Transform target)
    {
        _target = target;
    }

    public void Enable()
    {
        _rotate = true;
    }

    public void Disable()
    {
        _rotate = false;
    }

    private void LateUpdate()
    {
        if (_rotate)
        {
            transform.RotateAround(_target.position, transform.up, Input.GetAxis("Mouse X") * lerpSpeed);
       //     transform.RotateAround(_target.position, transform.right, -Input.GetAxis("Mouse Y") * lerpSpeed);
        }
    }
}
