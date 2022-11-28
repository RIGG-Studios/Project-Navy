using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ShipHealth : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float maxHealth;

    public float Health { get; private set; }
    public Ship Ship { get; private set; }
    
    private void Awake()
    {
        Ship = GetComponent<Ship>();
        Health = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        Health -= damageAmount;
        
        if (Health <= 0)
        {
            photonView.RPC("RPCDie", RpcTarget.All);

            NetworkPlayer owner = PhotonEventsManager.Instance.FindPlayerByActorID(Ship.OwnerActorNumber);

            if (owner != null)
            {
                owner.playerPhotonView.RPC("OnPlayerShipSink", owner.playerPhotonView.Owner);
            }
        }
    }

    [PunRPC]
    public void RPCDie()
    {
        Ship.ShipBuoyancy.enabled = false;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(Health);
        }
        else
        {
            Health = (float) stream.ReceiveNext();
        }
    }
}
