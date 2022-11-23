using Photon.Pun;
using UnityEngine;


[RequireComponent(typeof(BuoyancyObject))]
public class Ship : MonoBehaviourPun, IPunObservable
{
    public Transform playerSpawnPoint;
    
    public BuoyancyObject ShipBuoyancy { get; private set; }
    
    public int OwnerActorNumber { get; private set; }
    
    private void Awake()
    {
        ShipBuoyancy = GetComponent<BuoyancyObject>();
    }

    public void AssignShipToPlayer(int actorID)
    {
        OwnerActorNumber = actorID;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(OwnerActorNumber);
        }
        else
        {
            OwnerActorNumber = (int)stream.ReceiveNext();
        }
    }
}
