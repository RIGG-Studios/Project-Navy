using System;
using Photon.Pun;
using Photon.Realtime;
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

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (PhotonEventsManager.Instance.players.Count <= 0)
        {
            //game over
        //    PhotonNetwork.LeaveRoom();
        //    PhotonNetwork.LoadLevel(0);
        }
    }

    public void SpawnProjectile(Vector3 velocity, Vector3 pos, Quaternion rot, int ownerID)
    {
        CannonballController cannon = PhotonNetwork.Instantiate(projectilePrefab.name, pos, rot, 0).GetComponent<CannonballController>();

        if (cannon != null)
        {
            cannon.Init(ownerID, velocity);
        }
    }
    
    public void ExitMatch()
    {
        object[] package =
        {
            PhotonNetwork.LocalPlayer.ActorNumber
        };
        
        PhotonEventsManager.Instance.RaiseEvent(EventCodes.RemovePlayer, ReceiverGroup.MasterClient, package);
        
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }

    public void Spectate()
    {
        object[] package =
        {
            PhotonNetwork.LocalPlayer.ActorNumber
        };
        
        
        PhotonEventsManager.Instance.RaiseEvent(EventCodes.RemovePlayer, ReceiverGroup.MasterClient, package);

        PlayerSpawner.Instance.SpawnFreeCamPlayer();
    }
}
