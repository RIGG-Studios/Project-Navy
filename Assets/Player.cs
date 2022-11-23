using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Player : MonoBehaviourPun, IDamagable, IPunObservable
{
    [SerializeField] private float maxHealth = 100f;
    
    public string PlayerName { get; private set; }
    public int PlayerActorNumber { get; private set; }
    
    private float _currentHealth;
    private bool _canRecieveDamage;

    private Transform _spawnPoint;

    public int ActorID => PlayerActorNumber;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void SetupNetworkPlayer(Ship ship)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        photonView.RPC("SetInitialPlayerInfo", RpcTarget.AllBuffered, 
            PhotonNetwork.LocalPlayer.NickName, PhotonNetwork.LocalPlayer.ActorNumber);

        _spawnPoint = ship.playerSpawnPoint;
        
        //initialize package info
        object[] package =
        {
            PlayerName,
            PlayerActorNumber,
            photonView.ViewID
        };
        
        //send event with the package we created
        PhotonEventsManager.Instance.RaiseEvent(EventCodes.AddPlayer, ReceiverGroup.MasterClient, package);
    }

    [PunRPC]
    public void SetInitialPlayerInfo(string playerName, int actorID)
    {
        PlayerName = playerName;
        PlayerActorNumber = actorID;
        gameObject.name = PlayerName + (photonView.IsMine ? " (Local)" : " (Remote)");
    }
    
    private void Die()
    {
        if (photonView.IsMine)
        {
            transform.position = _spawnPoint.position;
            transform.rotation = _spawnPoint.rotation;

            _currentHealth = maxHealth;
            _canRecieveDamage = true;
        }
    }
    
    public void Damage(int attackerID, float damageAmount)
    {
        _currentHealth -= damageAmount;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_canRecieveDamage);
            stream.SendNext(_currentHealth);
            stream.SendNext(PlayerName);
            stream.SendNext(PlayerActorNumber);
        }
        else
        {
            _canRecieveDamage = (bool)stream.ReceiveNext();
            _currentHealth = (float)stream.ReceiveNext();
            PlayerName = (string)stream.ReceiveNext();
            PlayerActorNumber = (int)stream.ReceiveNext();
        }
    }
}
