using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swimming : MonoBehaviour
{
    [SerializeField] private UnderwaterChecker underwaterChecker;
    [SerializeField] private float buoyancy;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform cameraTransform;
    
    private PlayerController _playerController;
    private Rigidbody _rigidbody;

    private bool _stoppedMovement;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        bool underWater = underwaterChecker.IsUnderWater();
        
        if (!underWater)
        {
            if (_stoppedMovement)
            {
                _rigidbody.drag = 0;
                _playerController.canDoAnything = true;
                _playerController.canRecieveInput = true;
                _stoppedMovement = false;
            }
            
            return;
        }

        if (!_stoppedMovement)
        {
            _rigidbody.drag = 10f;
            _playerController.canDoAnything = false;
            _playerController.canRecieveInput = false;
            _stoppedMovement = true;
        }

        Vector3 buoyancy = -Physics.gravity * this.buoyancy;
        _rigidbody.AddForce(buoyancy, ForceMode.Acceleration);

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 velocity = transform.position +
                           (transform.right * x + cameraTransform.forward * y) * moveSpeed * Time.fixedDeltaTime;

        _rigidbody.MovePosition(velocity);
    }
}
