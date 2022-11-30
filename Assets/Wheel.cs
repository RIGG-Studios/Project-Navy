using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour, IInteractable
{
    public InteractTypes InteractTypes => InteractTypes.Wheel;
    
    public void LookAt()
    {
    }

    public void StopLookAt()
    {
    }

    public void Interact(PlayerController player)
    {
        player.ToggleShipControl(true);
    }
}
