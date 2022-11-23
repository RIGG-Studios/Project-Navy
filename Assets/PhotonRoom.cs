using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;

public class PhotonRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject eventsPrefab;
    [SerializeField] private GameObject damagePrefab;


    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient) 
            return;
        
        if (eventsPrefab != null)
        {
            PhotonNetwork.InstantiateRoomObject(eventsPrefab.name, Vector3.zero, quaternion.identity, 0);
        }

        if (damagePrefab != null)
        { 
            PhotonNetwork.InstantiateRoomObject(damagePrefab.name, Vector3.zero, quaternion.identity, 0);
        }
    }
}
