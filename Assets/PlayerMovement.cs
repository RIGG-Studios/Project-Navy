using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateTypes
{
    DefaultMovement,
    Swimming,
    ControllingShip
}

public class PlayerMovement : MonoBehaviour
{
    public State CurrentState { get; private set; }

    private DefaultPlayerMovement _defaultPlayerMovement;
    private Player _player;
    
    
    private void Awake()
    {
        _player = GetComponent<Player>();
        _defaultPlayerMovement = new DefaultPlayerMovement(_player);
    }

    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.OnUpdate();
        }
    }
    
    public void LateUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.OnLateUpdate();
        }
    }
    
    public void FixedUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.OnPhysicsUpdate();
        }
    }

    public void SwitchState(StateTypes state)
    {
        State newState = null;
        
        switch (state)
        {
            case StateTypes.Swimming:
                newState = null;
                break;
            
            case StateTypes.ControllingShip:
                newState = null;
                break;
            
            case StateTypes.DefaultMovement:
                newState = _defaultPlayerMovement;
                break;
        }

        if (newState != null)
        {
            if (CurrentState != null)
            {
                CurrentState.ExitState();
            }
            
            
            CurrentState.EnterState();
        }
    }
}
