using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoHitImpacts : MonoBehaviour
{
    private AudioSource _source;
    
    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlayHitImpactSound(AudioClip clip, float delay)
    {
        StartCoroutine(PlayClipWithDelay(clip, delay));
    }

    IEnumerator PlayClipWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        _source.clip = clip;
        _source.Play();
    }
}
