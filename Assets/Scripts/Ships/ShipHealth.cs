using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class ShipHealth : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float maxHealth;

    public float Health { get; private set; }

    private Ship _ship;
    
    private void Awake()
    {
        _ship = GetComponent<Ship>();
        Health = maxHealth;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
            Damage(50f);
    }

    public void Damage(float damageAmount)
    {
        Health -= damageAmount;
        
        if (Health <= 0)
        {
            photonView.RPC("RPCDie", RpcTarget.All);

            NetworkPlayer owner = PhotonEventsManager.Instance.FindPlayerByActorID(_ship.OwnerActorNumber);

            if (owner != null)
            {
                owner.playerPhotonView.RPC("OnPlayerShipSink", owner.playerPhotonView.Owner);
            }
        }
    }

    [PunRPC]
    public void RPCDie()
    {
        _ship.ShipBuoyancy.enabled = false;
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
