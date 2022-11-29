using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;


public class ShipNetworkManager : MonoBehaviourPun
{
    public static ShipNetworkManager Instance;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject shipPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public Ship RequestShip(int actorID)
    {
        Transform spawnPoint = FindSpawnPoint();
        
        Ship ship = PhotonNetwork.Instantiate(shipPrefab.name, spawnPoint.position, spawnPoint.rotation, 0)
            .GetComponent<Ship>();

        ship.AssignShipToPlayer(actorID);
   //     photonView.RPC("FillSpawnPoint", RpcTarget.All, i);

        return ship;
    }


    private Transform FindSpawnPoint()
    {
        int index = PhotonNetwork.LocalPlayer.ActorNumber;

        return spawnPoints[index];
    }
}
