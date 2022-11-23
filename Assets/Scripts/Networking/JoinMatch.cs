using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class JoinMatch : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private Slider matchTimeSlider;
    [SerializeField] private float timeBeforeDestroy = 20f;

    
    [HideInInspector]
    public int playersInRoom;
    [HideInInspector]
    public int maxPlayersInRoom;

    [HideInInspector]
    public int realPlayersInRoom;
    [HideInInspector]
    public int realMaxPlayersInRoom;
    
    private float _time;
    private bool _sentCall;

    private void Awake()
    {
        _time = timeBeforeDestroy;
        matchTimeSlider.maxValue = timeBeforeDestroy;
        matchTimeSlider.minValue = 0.0f;
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                _time -= Time.deltaTime;

                matchTimeSlider.value = _time;
                photonView.RPC("SendProgress", RpcTarget.All, _time);

                if (_time < 0.0f)
                {
                    photonView.RPC("TimeOutMaster", RpcTarget.All);
                }

                if (playersInRoom > maxPlayersInRoom && !_sentCall)
                {
                    photonView.RPC("LoadSceneAll", RpcTarget.All);
                    _sentCall = true;
                }
            }

            if (!photonView.IsMine)
            {
                playersInRoom = realPlayersInRoom;
                maxPlayersInRoom = realMaxPlayersInRoom;
            }
        }
    }

    [PunRPC]
    public void SendProgress(float totalProgress)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            matchTimeSlider.value = totalProgress;
        }
    }

    public void AcceptMatch()
    {
        photonView.RPC("AcceptMaster", RpcTarget.MasterClient);
    }

    
    [PunRPC]
    public void AcceptMaster()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("AcceptAll", RpcTarget.All);
        }
    }

    [PunRPC]
    public void AcceptAll()
    {
        playersInRoom++;
    }

    [PunRPC]
    public void TimeOutMaster()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("TimeOutAll", RpcTarget.All);
        }
        
    }
    
    [PunRPC]
    public void TimeOutAll()
    {
        _time = timeBeforeDestroy;
        joinPanel.SetActive(false);

        MatchMaking.Instance.ResetMatchMaking();
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void LoadSceneAll()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) 
        {
            stream.SendNext(playersInRoom);
            stream.SendNext (maxPlayersInRoom);
        }
        else
        {
            realPlayersInRoom = (int)stream.ReceiveNext();
            realMaxPlayersInRoom = (int)stream.ReceiveNext();
        }
    }
}
