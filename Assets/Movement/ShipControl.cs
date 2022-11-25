using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class ShipControl : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject remoteBody;
    public float turnSpeed = 5f;
    public float maxSpeed = 10f;
    public float acceleration = 0.2f;

    private bool _controlling;
    private bool _InTrigger;
    
    private PlayerController _playerController;
    private Player _player;

    private float _x, _y;


    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        _y = Input.GetAxis("Vertical");
        _x = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.F) && _InTrigger && !_controlling)
        {
            _controlling = true;
            _playerController.canRecieveInput = false;
            playerCamera.gameObject.SetActive(false);
            _player.PlayerShip.ToggleCamera(true);
            remoteBody.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.F) && _controlling)
        {
            _controlling = false;
            _playerController.canRecieveInput = true;
            playerCamera.gameObject.SetActive(true);
            _player.PlayerShip.ToggleCamera(false);
            remoteBody.SetActive(false);
        }

    }

    void FixedUpdate()
    {
        if (_controlling)
        {
            float forwardVel = _y * acceleration;
            float horizontalVel = _x * turnSpeed;

            forwardVel = Mathf.Clamp(forwardVel, 0, maxSpeed);

            _player.PlayerShip.MoveShip(forwardVel);
            _player.PlayerShip.RotateShip(horizontalVel);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _InTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _InTrigger = false;
    }
}