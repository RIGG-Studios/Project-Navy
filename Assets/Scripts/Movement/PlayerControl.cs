using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    Rigidbody body;
    ControlScheme controls;

    public GameObject fpsCamera;
    //private GameObject ship;

    float moveSpeed = 10f;
    //float climbSpeed = 3;
    //float swimSpeed = 3;
    float mouseSens = 0.1f;

    private InputAction move;
    private InputAction jump;
    private InputAction look;

    Vector2 moveDirection = Vector2.zero;
    Vector2 lookDirection = Vector2.zero;
    Transform facingAngle;
    float lookVertical;

    bool isGrounded = true;
    //bool isClimbing;
    //bool isSwimming;
    bool isSteering = false;

    void Awake()
    {
        controls = new ControlScheme();
        body = GetComponent<Rigidbody>();
        facingAngle = fpsCamera.transform;

        //Initialize inputs
        move = controls.Player.Movement;
        move.Enable();
        jump = controls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;
        look = controls.Player.Look;
        look.Enable();

        //Lock cursor for first-person view
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        lookDirection = look.ReadValue<Vector2>();
    }

    // Using FixedUpdate() here instead of Update(), because this is a multiplayer game and I don't want to risk any framerate discrepencies fucking with movement
    void FixedUpdate()
    {
        //If the player isn't steering, move them. If they are, move the ship.
        if(!isSteering)
        {
            Move();
        } else
        {
            Steer();
        }
        //CheckIfGrounded();
    }

    void LateUpdate()
    {
        Look();
    }

    //Handles player movement (walking/running).
    void Move()
    {
        Vector3 movement = new Vector3(moveDirection.x, 0f, moveDirection.y);
        movement = facingAngle.forward * movement.z + facingAngle.right * movement.x;

        //MAJOR BUG: Movement doesn't work right unless you're jumping. I poked around with the numbers a bit, and it seems like the issue might be related to the Rigidbody's mass? Not sure how to fix; will experiment.
        body.AddForce(movement * moveSpeed);
    }

    //Handles player turning (camera movement/looking around).
    void Look()
    {
        transform.Rotate(Vector3.up * lookDirection.x * mouseSens);

        lookVertical -= lookDirection.y * mouseSens;
        lookVertical = Mathf.Clamp(lookVertical, -90, 90);
        fpsCamera.transform.eulerAngles = new Vector3(lookVertical, fpsCamera.transform.eulerAngles.y, fpsCamera.transform.eulerAngles.z);
    }

    //Handles jumping; only works if the player is 'grounded'.
    void Jump(InputAction.CallbackContext context)
    {
        if(isGrounded)
        {
            body.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    //Used instead of Move() if the player is steering a ship; passes inputs to the ship so it can move accordingly (see ShipControl.cs).
    void Steer(){ }

    //Checks if the player is touching a 'ground' surface; if so, they can jump. There's definitely a better way to do this, but this should do for now.
    private void OnCollisionEnter(Collision thing)
    {
        if(thing.gameObject.layer == 3)
        {
            isGrounded = true;
        }
    }
}