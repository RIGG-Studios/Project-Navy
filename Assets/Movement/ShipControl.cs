using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour
{
    float forwardVelocity = 0f;
    float horizontInput;
    //Unsure about these numbers; will need testing
    float turnSpeed = 5f;
    float maxSpeed = 10f;
    float acceleration = 0.2f;

    bool moving;
    
    Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //Accelerate or Decelerate based on whether the player is inputting forwards movement
        if(moving)
        {
            Accelerate();
        } else
        {
            Decelerate();
        }
        //Limit speed to maximum
        forwardVelocity = Mathf.Clamp(forwardVelocity, 0, maxSpeed);

        //move and rotate based on velocity
        Vector3 rotationVar = Vector3.up * horizontInput;
        Quaternion angleRotation = Quaternion.Euler(rotationVar * Time.fixedDeltaTime);

        body.AddForce(transform.forward * forwardVelocity, ForceMode.Force);
        body.MoveRotation(body.rotation * angleRotation);
    }

    //These functions increase or decrease the ship's speed. They reason they're functions (despite being so short) is so that ship speed can be modified by other classes if necessary.
    public void Accelerate()
    {
        forwardVelocity += acceleration;
    }
    public void Decelerate()
    {
        forwardVelocity -= acceleration;
    }

    //Turns the ship; called when the player presses A or D while steering the ship
    public void Turn()
    {
        horizontInput += turnSpeed;
    }

    //Checks if the ship is being moved forwards; called when the player presses/releases W while steering the ship
    public void SetMoving(bool input)
    {
        moving = input;
    }
}