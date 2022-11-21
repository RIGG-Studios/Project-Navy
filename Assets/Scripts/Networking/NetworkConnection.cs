using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkConnection : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject sceneCamera;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        ConnectToPhoton();
    }

    public void ConnectToPhoton()
    {
        try
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        catch
        {
            Debug.Log("Error connecting to photon servers");
        }
    }
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master");

        PhotonNetwork.LocalPlayer.NickName = "Player " + Random.Range(0, 1000);
        
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to lobby");

        string roomName = "Room";
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
    
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " Has joined the room!");

       // PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
       GameManager.Instance.SpawnPlayerShip();
        sceneCamera.SetActive(false);
    }
}
