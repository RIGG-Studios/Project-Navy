using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultPlayerMovement : State
{
    public DefaultPlayerMovement(Player player) : base(StateTypes.DefaultMovement, player)
    {
        Player = player;
    }


    public override void OnUpdate()
    {
        
    }

    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {
        
    }
}
