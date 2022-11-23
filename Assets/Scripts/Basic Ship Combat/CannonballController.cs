using System;
using UnityEngine;

public class CannonballController : MonoBehaviour
{
    public float damage;
    public string shipTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(shipTag))
        {
            Debug.Log("Do ship damage stuff here");
        }
    }
}
