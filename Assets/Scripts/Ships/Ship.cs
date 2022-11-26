using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;


[RequireComponent(typeof(BuoyancyObject))]
public class Ship : MonoBehaviourPun, IPunObservable
{
    public ShipCamera shipCamera;
    public Transform playerSpawnPoint;
    public Transform rudderTransform;

    public BuoyancyObject ShipBuoyancy { get; private set; }
    public int OwnerActorNumber { get; private set; }
    public Rigidbody Rigidbody { get; private set; }


    private List<CannonController> cannons = new List<CannonController>();
    
    private void Awake()
    {
        ShipBuoyancy = GetComponent<BuoyancyObject>();
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        shipCamera.Init(transform);

        CannonController[] cannons = GetComponentsInChildren<CannonController>();

        if (cannons.Length > 0)
        {
            for (int i = 0; i < cannons.Length; i++)
            {
                cannons[i].Init(this, i);
                this.cannons.Add(cannons[i]);
            }
        }
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

    public void OnCannonFired(int cannonID, Vector3 pos, Quaternion rot)
    {
        photonView.RPC("RPCEffects", RpcTarget.All, cannonID, pos, rot);
    }
    
    [PunRPC]
    public void RPCEffects(int cannonID, Vector3 spawnPoint, Quaternion rot)
    {
        CannonController cannon = FindCannon(cannonID);

        if (cannon == null)
        {
            Debug.Log("couldnt find cannon");
            return;
        }
        
        //audio
        cannon.source.clip = cannon.fireSounds[UnityEngine.Random.Range(0, cannon.fireSounds.Length)];
        cannon.source.Play();
        
        //effects
        VisualEffect vfx = Instantiate(cannon.muzzleFlash, spawnPoint, rot)
            .GetComponent<VisualEffect>();
            
        vfx.Play();
    }

    private CannonController FindCannon(int id)
    {
        for (int i = 0; i < cannons.Count; i++)
        {
            if (id == cannons[i].CannonID)
            {
                return cannons[i];
            }
        }

        return null;
    }
}
