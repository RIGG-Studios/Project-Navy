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
        Transform spawnPoint = FindSpawnPoint();

        Ship ship = PhotonNetwork.Instantiate(shipPrefab.name, spawnPoint.position, spawnPoint.rotation, 0)
            .GetComponent<Ship>();

        ship.AssignShipToPlayer(actorID);

        return ship;
    }

    private Transform FindSpawnPoint()
    {
        Ship[] ships = FindObjectsOfType<Ship>();

        if (ships.Length > 0)
        {
            for (int i = 0; i < ships.Length; i++)
            {
                for (int z = 0; z < shipSpawnPoints.Length; z++)
                {
                    float dist = (ships[i].transform.position - shipSpawnPoints[i].position).magnitude;

                    if (dist >= 15f)
                    {
                        return shipSpawnPoints[z];
                    }
                }
            }
        }
        else
        {
            return shipSpawnPoints[Random.Range(0, shipSpawnPoints.Length)];
        }

        return shipSpawnPoints[0];
    }
}
