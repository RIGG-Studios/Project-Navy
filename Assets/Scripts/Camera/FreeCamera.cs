using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    [SerializeField] private float lookSpeed;
    [SerializeField] private float moveSpeed;

    private Vector2 _rotation;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _rotation.y += Input.GetAxis ("Mouse X");
        _rotation.x += -Input.GetAxis ("Mouse Y");
        transform.eulerAngles = (Vector2)_rotation * lookSpeed;


        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        transform.position += (transform.right * x + transform.forward * y) * moveSpeed;
    }
}
