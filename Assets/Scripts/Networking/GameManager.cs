using System;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnPlayerShip();
    }

    public void SpawnPlayerShip()
    {
        /*/
        if (success)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, ship.playerSpawnPoint.position, ship.playerSpawnPoint.rotation,
                0);
        }
        /*/
    }
}
