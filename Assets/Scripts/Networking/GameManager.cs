using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private Transform[] shipSpawnPoints;
    [SerializeField] private GameObject shipPrefab;
    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnPlayerShip()
    {
        Transform spawnPoint = shipSpawnPoints[Random.Range(0, shipSpawnPoints.Length)];

        Ship ship = PhotonNetwork.Instantiate(shipPrefab.name, spawnPoint.position, spawnPoint.rotation, 0)
            .GetComponent<Ship>();

        bool success = ship != null;

        if (success)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, ship.playerSpawnPoint.position, ship.playerSpawnPoint.rotation,
                0);
        }
    }

}
