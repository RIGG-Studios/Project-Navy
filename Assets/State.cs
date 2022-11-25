using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected StateTypes StateType;
    protected Player Player;

    protected State(StateTypes type, Player player)
    {
        StateType = type;
        this.Player = player;
    }
    
    public abstract void EnterState();

    public abstract void ExitState();
    
    public virtual void OnUpdate() { }
    
    public virtual void OnLateUpdate() { }
    
    public virtual void OnPhysicsUpdate() { }
}
