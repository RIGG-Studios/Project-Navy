using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementTest : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform cameraTrans;
    [SerializeField] private float mouseSpeed;


    private Rigidbody _rigidbody;

    private float _x;
    private float _y;

    private float _xRot;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        _x = Input.GetAxis("Horizontal");
        _y = Input.GetAxis("Vertical");
        
    }

    private void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

        _xRot -= mouseY;
        _xRot = Mathf.Clamp(_xRot, -90f, 90f);
        
        cameraTrans.localRotation = Quaternion.Euler(_xRot, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForceAtPosition(Physics.gravity, transform.position, ForceMode.Acceleration);
        
        Vector3 playerVelocity = transform.position +
                                 (_x * transform.right + _y * transform.forward) *
                                 (Time.fixedDeltaTime * movementSpeed);
        
        _rigidbody.MovePosition(playerVelocity);
    }
}
