using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance;
    
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject sceneCamera;
    [SerializeField] private Transform[] playerSpawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(SpawnPlayerWithDelay(2.0f));
    }

    private IEnumerator SpawnPlayerWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Ship ship = ShipNetworkManager.Instance.RequestShip(PhotonNetwork.LocalPlayer.ActorNumber);

        if (ship != null)
        {
            ship.AssignShipToPlayer(PhotonNetwork.LocalPlayer.ActorNumber);
            
            Player player = PhotonNetwork.Instantiate(playerPrefab.name, ship.playerSpawnPoint.position, ship.playerSpawnPoint.rotation, 0)
                .GetComponent<Player>();
            
            player.SetupNetworkPlayer(ship);
            sceneCamera.SetActive(false);
        }
        
    }

    public Transform GetRandomSpawnPoint()
    {
        return playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)];
    }
}
