using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour
{
    float forwardVelocity = 0f;
    float horizontInput;
    //Unsure about these numbers; will need testing
    public float turnSpeed = 5f;
    public float maxSpeed = 10f;
    public float acceleration = 0.2f;

    bool moving;
    private bool _controlling;
    private bool _InTrigger;
    
    Rigidbody body;
    private PlayerController _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<PlayerController>();
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (_InTrigger)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _controlling = !_controlling;
                _player.canDoAnything = !_controlling;
            }
        }
        
        if (_controlling)
        {
            float y = Input.GetAxis("Vertical");
            float x = Input.GetAxis("Horizontal");

            
            
            //Accelerate or Decelerate based on whether the player is inputting forwards movement
            if (moving)
            {
                Accelerate(y);
            }
            else
            {
                Decelerate(y);
            }
            
            Turn(x);

            //Limit speed to maximum
            forwardVelocity = Mathf.Clamp(forwardVelocity, 0, maxSpeed);

            //move and rotate based on velocity
            Vector3 rotationVar = Vector3.up * horizontInput;
            Quaternion angleRotation = Quaternion.Euler(rotationVar * Time.fixedDeltaTime);

            body.AddForce(transform.forward * forwardVelocity, ForceMode.Force);
            body.MoveRotation(body.rotation * angleRotation);
        }
    }

    //These functions increase or decrease the ship's speed. They reason they're functions (despite being so short) is so that ship speed can be modified by other classes if necessary.
    public void Accelerate(float y)
    {
        forwardVelocity += y * acceleration;
    }
    public void Decelerate(float y)
    {
        forwardVelocity -= y * acceleration;
    }

    //Turns the ship; called when the player presses A or D while steering the ship
    public void Turn(float x)
    {
        horizontInput += x* turnSpeed;
    }

    //Checks if the ship is being moved forwards; called when the player presses/releases W while steering the ship
    public void SetMoving(bool input)
    {
        moving = input;
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