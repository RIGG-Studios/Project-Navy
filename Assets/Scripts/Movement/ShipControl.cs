using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class ShipControl : MonoBehaviour
{
    
    public Camera playerCamera;
    public GameObject remoteBody;
    public float lookDist;
    public float turnSpeed = 5f;
    public float maxSpeed = 10f;
    public float acceleration = 0.2f;

    private bool _controlling;
    
    private PlayerController _playerController;
    private Player _player;

    private float _x, _y;
    private bool _looking;

    private float _fowardVelocity;
    private bool _isFPCamera;

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, lookDist))
        {
            _looking = hit.collider.CompareTag("WheelTrigger");

            if (_looking && !_controlling)
            {
                _player.ToggleInteractHelper(true);
            }
        }
        else
        {
            _player.ToggleInteractHelper(false);
            _looking = false;
        }
        
        _y = Input.GetAxis("Vertical");
        _x = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.F) && _looking && !_controlling)
        {
            _controlling = true;
            _playerController.canRecieveInput = false;
            playerCamera.gameObject.SetActive(false);
            _player.PlayerShip.ToggleCamera(true);
            remoteBody.SetActive(true);
            _player.ToggleInteractHelper(false);
            _player.ToggleShipControlUI(true);
            _playerController.musketController.musketAnimator.gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && _controlling)
        {
            _controlling = false;
            _playerController.canRecieveInput = true;
            playerCamera.gameObject.SetActive(true);
            _player.PlayerShip.ToggleCamera(false);
            remoteBody.SetActive(false);
            _player.ToggleShipControlUI(false);
            _playerController.musketController.musketAnimator.gameObject.SetActive(true);
        }

        if (_controlling)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _isFPCamera = !_isFPCamera;

                if (_isFPCamera)
                {
                    playerCamera.gameObject.SetActive(true);
                    _player.PlayerShip.ToggleCamera(false);
                }
                else
                {
                    playerCamera.gameObject.SetActive(false);
                    _player.PlayerShip.ToggleCamera(true);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (_controlling)
        { 
            _fowardVelocity = _y * acceleration;
            float horizontalVel = _x * turnSpeed;

            _fowardVelocity = Mathf.Clamp(_fowardVelocity, 0.1f, maxSpeed);

            _player.PlayerShip.RotateShip(horizontalVel);
        }
        
        _player.PlayerShip.MoveShip(_fowardVelocity);

    }
}