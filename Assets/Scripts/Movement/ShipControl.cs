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

    public bool controlling { get; private set; }
    
    private PlayerController _playerController;
    private MusketController _musketController;
    private Player _player;

    private float _x, _y;
    private bool _looking;

    private float _fowardVelocity;
    private bool _isFPCamera;

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _musketController = GetComponent<MusketController>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, lookDist))
        {
            _looking = hit.collider.CompareTag("WheelTrigger");

            if (_looking && !controlling)
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

        if (Input.GetKeyDown(KeyCode.F) && _looking && !controlling)
        {
            controlling = true;
            _playerController.canRecieveInput = false;
            _musketController.canDoAnything = false;
            playerCamera.gameObject.SetActive(false);
            _player.PlayerShip.ToggleCamera(true);
            remoteBody.SetActive(true);
            _player.ToggleInteractHelper(false);
            _player.ToggleShipControlUI(true);
            _playerController.musketController.musketAnimator.gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && controlling)
        {
            controlling = false;
            _playerController.canRecieveInput = true;
            _musketController.canDoAnything = true;
            playerCamera.gameObject.SetActive(true);
            _player.PlayerShip.ToggleCamera(false);
            remoteBody.SetActive(false);
            _player.ToggleShipControlUI(false);
            _playerController.musketController.musketAnimator.gameObject.SetActive(true);
        }

        if (controlling)
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

    public void Reset()
    {
        if (!controlling)
            return;
        
        controlling = false;
        _playerController.canRecieveInput = true;
        _musketController.canDoAnything = true;
        playerCamera.gameObject.SetActive(true);
        _player.PlayerShip.ToggleCamera(false);
        remoteBody.SetActive(false);
        _player.ToggleShipControlUI(false);
        _playerController.musketController.musketAnimator.gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        if (controlling)
        { 
            _fowardVelocity = _y * acceleration;
            float horizontalVel = _x * turnSpeed;

            _fowardVelocity = Mathf.Clamp(_fowardVelocity, 0.0f, maxSpeed);

            _player.PlayerShip.RotateShip(horizontalVel);
        }
        
        _player.PlayerShip.MoveShip(_fowardVelocity);

    }
}