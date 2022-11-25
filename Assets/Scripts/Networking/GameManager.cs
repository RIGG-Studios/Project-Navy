using System;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private GameObject projectilePrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnProjectile(Vector3 velocity, Vector3 pos, Quaternion rot)
    {
        Rigidbody rb = PhotonNetwork.Instantiate(projectilePrefab.name, pos, rot, 0).GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(velocity, ForceMode.Impulse);
        }
    }
}
