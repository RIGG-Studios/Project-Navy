using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerNetwork : MonoBehaviourPun
{
    [Header("Remote Player Disables")]
    
    [SerializeField] private Behaviour[] componentsToDisable;
    [SerializeField] private GameObject[] gameObjectsToDisable;

    [Header("Local Player Disables")]
    
    [SerializeField] private GameObject[] localGameObjectsToDisable;

    
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
        else
        {
            foreach (GameObject g in localGameObjectsToDisable)
            {
                g.SetActive(false);
            }
        }
    }
}
