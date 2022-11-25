using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RemotePhysicsUpdater : MonoBehaviourPun
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            _rigidbody.AddForceAtPosition(Physics.gravity, transform.position, ForceMode.Acceleration);
        }
    }
}
