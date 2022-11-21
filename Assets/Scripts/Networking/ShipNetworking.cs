using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShipNetworking : MonoBehaviourPun
{
    [SerializeField] private Behaviour[] componentsToDisable;
    [SerializeField] private GameObject[] gameObjectsToDisable;
    
    
    private void Start()
    {
        if (!photonView.IsMine)
        {
            foreach (GameObject g in gameObjectsToDisable)
            {
                g.SetActive(false);
            }

            foreach (Behaviour b in componentsToDisable)
            {
                b.enabled = false;
            }
        }
    }
}
