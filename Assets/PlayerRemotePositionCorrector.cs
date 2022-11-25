using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Smooth;
using UnityEngine;

public class PlayerRemotePositionCorrector : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float correctionForce;
    
    private Rigidbody _rigidbody;
    private Player _player;

    private Vector3 _nextPos;
    private Vector3 _nextVel;
    private Quaternion _nextRot;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            return;
        }

        Vector3 playerVelocity = _nextVel;
        Vector3 shipVel = _player.PlayerShip.ShipBuoyancy.rigidBody.velocity;

        Vector3 nextVelocity = _nextPos + (playerVelocity - shipVel);
     //   _rigidbody.MovePosition(nextVelocity);

        _rigidbody.position = Vector3.Lerp(_rigidbody.position, nextVelocity, Time.fixedDeltaTime);
        _rigidbody.rotation = Quaternion.Lerp(_rigidbody.rotation, _nextRot, Time.fixedDeltaTime);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_rigidbody.position);
            stream.SendNext(_rigidbody.velocity);
            stream.SendNext(_rigidbody.rotation);
        }
        else
        {
            _nextPos = (Vector3) stream.ReceiveNext();
            _nextVel = (Vector3) stream.ReceiveNext();
            _nextRot = (Quaternion) stream.ReceiveNext();
        }
    }
}
