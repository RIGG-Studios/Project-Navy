using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [HideInInspector] public Transform poolTransform;
    

    public void ReturnToPool()
    {
        if (poolTransform == null)
        {
            Destroy(gameObject);
            return;
        }
        
        gameObject.SetActive(false);
        transform.SetParent(poolTransform, false);
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.eulerAngles = Vector3.zero;
    }
}
