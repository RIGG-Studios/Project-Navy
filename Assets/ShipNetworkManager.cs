using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;


public class ShipNetworkManager : MonoBehaviourPun
{
    public static ShipNetworkManager Instance; 
    
    [SerializeField] private Transform[] shipSpawnPoints;
    [SerializeField] private GameObject shipPrefab;

    
    private void Awake()
    {
        Instance = this;
    }

    public Ship RequestShip(int actorID)
    {
        Transform spawnPoint = shipSpawnPoints[Random.Range(0, shipSpawnPoints.Length)];

        Ship ship = PhotonNetwork.Instantiate(shipPrefab.name, spawnPoint.position, spawnPoint.rotation, 0)
            .GetComponent<Ship>();
        
        ship.AssignShipToPlayer(actorID);
  //      player.playerPhotonView.RPC("AssignShip", RpcTarget.AllBuffered);
        return ship;
    }
}
