using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class PlayerShips
{
    public Ship ship;
    public PhotonView player;
}

public class ShipNetworkManager : MonoBehaviourPun
{
    public static ShipNetworkManager Instance; 
    
    [SerializeField] private Transform[] shipSpawnPoints;
    [SerializeField] private GameObject shipPrefab;

    public List<PlayerShips> ships = new List<PlayerShips>();
    
    private void Awake()
    {
        Instance = this;
    }

    public Ship RequestShip(NetworkPlayer player)
    {
        Transform spawnPoint = shipSpawnPoints[Random.Range(0, shipSpawnPoints.Length)];

        Ship ship = PhotonNetwork.Instantiate(shipPrefab.name, spawnPoint.position, spawnPoint.rotation, 0)
            .GetComponent<Ship>();
        
        photonView.RPC("AddShip", RpcTarget.AllBuffered, player.actorID);
        
        return ship;
    }
}
