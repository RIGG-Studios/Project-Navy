using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallHolder : MonoBehaviour, IInteractable
{
    public InteractTypes InteractTypes => InteractTypes.CannonBallHolder;

    public void LookAt()
    {
        Debug.Log("ahh");
    }

    public void StopLookAt()
    {
    }

    public void Interact(PlayerController player)
    {
        player.GiveCannonBall();
    }
}
