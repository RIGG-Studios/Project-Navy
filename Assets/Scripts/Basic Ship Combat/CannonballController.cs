using System;
using UnityEngine;

public class CannonballController : MonoBehaviour
{
    public float damage;
    public string shipTag;
    public string waterTag;
    public AudioSource source;
    public AudioClip[] hitShipSounds;
    public AudioClip[] hitWaterSounds;

    private bool _hit;

    private void OnCollisionEnter(Collision other)
    {
        if(_hit)return;
        
        if (other.gameObject.CompareTag(shipTag))
        {
            _hit = true;
            source.clip = hitShipSounds[UnityEngine.Random.Range(0, hitShipSounds.Length)];
            source.Play();
            Debug.Log("Do ship damage stuff here");
            Destroy(gameObject, 2.0f);
        }
        else if(other.gameObject.CompareTag(waterTag))
        {
            _hit = true;
            source.clip = hitWaterSounds[UnityEngine.Random.Range(0, hitWaterSounds.Length)];
            source.Play();
            Destroy(gameObject, 2.0f);
        }
    }
}
