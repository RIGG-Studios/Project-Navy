using System;
using Photon.Pun;
using UnityEngine;


[RequireComponent(typeof(BuoyancyObject))]
public class Ship : MonoBehaviourPun, IPunObservable
{
    public ShipCamera shipCamera;
    public Transform playerSpawnPoint;
    public Transform rudderTransform;

    public BuoyancyObject ShipBuoyancy { get; private set; }
    public int OwnerActorNumber { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    
    private void Awake()
    {
        ShipBuoyancy = GetComponent<BuoyancyObject>();
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        shipCamera.Init(transform);
    }

    public void AssignShipToPlayer(int actorID)
    {
        OwnerActorNumber = actorID;
    }

    public void ToggleCamera(bool state)
    {
        shipCamera.gameObject.SetActive(state);

        if (state)
        {
            shipCamera.Enable();
        }
        else
        {
            shipCamera.Disable();
        }
    }

    public void MoveShip(float velocity)
    {
        Rigidbody.AddForce(transform.forward * velocity, ForceMode.Acceleration);
    }

    public void RotateShip(float turnAmount)
    {
       Rigidbody.AddForceAtPosition(turnAmount * -rudderTransform.right, rudderTransform.position);
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
