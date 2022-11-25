using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class CannonballController : MonoBehaviour
{
    public float damage;
    public float waterVelocityMultiplier;
    public string shipTag;
    public AudioSource source;
    public AudioClip[] hitShipSounds;
    public AudioClip[] hitWaterSounds;

    private bool _hit;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
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
        if(_hit)return;
        
        if (other.gameObject.CompareTag(shipTag))
        {
            _hit = true;
            source.clip = hitShipSounds[UnityEngine.Random.Range(0, hitShipSounds.Length)];
            source.Play();
            other.collider.GetComponent<ShipVitalPoint>().Damage(PhotonEventsManager.Instance.LocalPlayer.actorID, damage);
            Destroy(gameObject, 2.0f);
        }
    }
}
