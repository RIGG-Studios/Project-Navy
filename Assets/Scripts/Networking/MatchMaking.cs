using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MatchMaking : MonoBehaviourPunCallbacks
{
    public static MatchMaking Instance;

    [SerializeField] private int maxPlayersPerRoom = 2;

    [Space]
    
    [SerializeField] private JoinMatch joinMatch;
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject joinMatchPanel;
    [SerializeField] private GameObject[] disableOnMatchFound;
    
    public float TimeInMinutes { get; private set; }
    public float TimeInSeconds { get; private set; }
    
    
    [HideInInspector]
    public bool roomCreated;
    [HideInInspector]
    public bool roomFull;
    
    [HideInInspector]
    public int playersInRoomCount;
    [HideInInspector]
    public int maxPlayersInRoomCount;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (roomCreated)
            {
                if (!joinMatchPanel.activeSelf)
                {
                    if (TimeInSeconds >= 60f)
                    {
                        TimeInSeconds = 0f;
                        TimeInMinutes++;
                    }

                    TimeInSeconds += Time.deltaTime;

                    timerText.text = TimeInMinutes.ToString("0") + ":" + TimeInSeconds.ToString("00");
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    if (!roomFull)
                    {
                        PhotonNetwork.CurrentRoom.IsVisible = true;

                        maxPlayersInRoomCount = PhotonNetwork.CurrentRoom.MaxPlayers;
                        playersInRoomCount = PhotonNetwork.CurrentRoom.PlayerCount;
                        
                        if (playersInRoomCount >= maxPlayersInRoomCount)
                        {
                            roomFull = true;
                            PhotonNetwork.CurrentRoom.IsVisible = false;
                        }
                    }
                    else
                    {
                        photonView.RPC("AcceptMatchAll", RpcTarget.All);
                    }
                }
            }
            else
            {
                TimeInMinutes = 0;
                TimeInSeconds = 0;
            }
        }
    }

    [PunRPC]
    public void AcceptMatchAll()
    {
        joinMatchPanel.SetActive(true);

        for (int i = 0; i < disableOnMatchFound.Length; i++)
        {
            disableOnMatchFound[i].SetActive(false);
        }
    }

    public void ResetMatchMaking()
    {
        roomCreated = false;
        roomFull = false;
        for (int i = 0; i < disableOnMatchFound.Length; i++)
        {
            disableOnMatchFound[i].SetActive(true);
        }
        
        CreateSearch();
    }

    public void CreateSearch()
    {
        if (!roomCreated)
        {
            PhotonNetwork.JoinRandomRoom ();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxPlayersPerRoom;

        PhotonNetwork.CreateRoom ("TestMap" + "@" + Guid.NewGuid().ToString("N"), roomOptions, null);

        joinMatch.maxPlayersInRoom = maxPlayersPerRoom;
        roomCreated = true;
    }

    public override void OnJoinedRoom()
    {
        roomCreated = true;
    }

    public void CancelGame()
    {
        PhotonNetwork.LeaveRoom();
    }
}
