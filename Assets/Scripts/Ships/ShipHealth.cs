using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.VFX;

public class ShipHealth : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float maxHealth;
    [SerializeField] private VisualEffect[] destroyEffects;
    [SerializeField] private float delay;
    
    public float Health { get; private set; }
    public Ship Ship { get; private set; }
    
    private void Awake()
    {
        Ship = GetComponent<Ship>();
        Health = maxHealth;
    }
    
    public void Damage(float damageAmount)
    {
        Health -= damageAmount;
        
        if (Health <= 0)
        {
            photonView.RPC("RPCDie", RpcTarget.All);

            Ship.OnDie();
            NetworkPlayer owner = PhotonEventsManager.Instance.FindPlayerByActorID(Ship.OwnerActorNumber);

            if (owner != null)
            {
                owner.playerPhotonView.RPC("OnPlayerShipSink", owner.playerPhotonView.Owner);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Damage(100f);
        }
    }

    [PunRPC]
    public void RPCDie()
    {
        Ship.ShipBuoyancy.enabled = false;

        for (int i = 0; i < destroyEffects.Length; i++)
        {
            destroyEffects[i].Play();
        }
    }

    private IEnumerator PlayDestroyEffects()
    {
        float t = 0.0f;

        for (int i = 0; i < destroyEffects.Length; i++)
        {
            while (t <= delay && i < destroyEffects.Length) 
            {
                t += Time.deltaTime;
                destroyEffects[i].Play();

                yield return null;
            }
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(Health);
        }
        else
        {
            Health = (float) stream.ReceiveNext();
        }
    }
}
