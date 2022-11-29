using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.VFX;

public class CannonballController : MonoBehaviour, IPunObservable
{
    public GameObject hitEffect;
    public float damage;
    public float waterVelocityMultiplier;
    public float shipImpactForce;
    public AudioSource source;
    public AudioClip[] hitShipSounds;
    public AudioClip[] hitWaterSounds;

    private bool _hit;
    private Rigidbody _rigidbody;

    private int _ownerID;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Init(int ownerID, Vector3 velocity)
    {
        _ownerID = ownerID;
        _rigidbody.AddForce(velocity, ForceMode.Impulse);
    }

    private void Update()
    {
        float waterHeight = Ocean.Instance.GetWaterHeightAtPosition(transform.position);

        if (transform.position.y < waterHeight && !_hit)
        {
            _hit = true;
            source.clip = hitWaterSounds[UnityEngine.Random.Range(0, hitWaterSounds.Length)];
            source.Play();
            _rigidbody.drag += Time.deltaTime * waterVelocityMultiplier;

            StartCoroutine(DestroyCannonBall());
        }
    }

    private IEnumerator DestroyCannonBall()
    {
        yield return new WaitForSeconds(4.5f);
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(_hit)
            return;
        
        if (other.gameObject.TryGetComponent(out ShipVitalPoint vitalPoint))
        {
            _hit = true;
            
            //audio
            source.clip = hitShipSounds[UnityEngine.Random.Range(0, hitShipSounds.Length)];
            source.Play();
            
            //effects
            VisualEffect vfx = Instantiate(hitEffect, transform.position, transform.rotation)
                .GetComponent<VisualEffect>();
            
            vfx.Play();

            if (vitalPoint.ActorID == _ownerID)
                return;
            
            //damage
            
            
            vitalPoint.Damage(PhotonEventsManager.Instance.LocalPlayer.actorID, damage);

            if (vitalPoint.ShipHealth != null)
            {
                vitalPoint.ShipHealth.Ship.Rigidbody.AddForce(_rigidbody.velocity * shipImpactForce, ForceMode.Force);
            }
            
            PhotonEventsManager.Instance.LocalPlayer.playerPhotonView.RPC("OnPlayerDamagedShip",  PhotonEventsManager.Instance.LocalPlayer.playerPhotonView.Owner);
            Destroy(gameObject, 2.0f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_ownerID);
        }
        else
        {
            _ownerID = (int) stream.ReceiveNext();
        }
    }
}
