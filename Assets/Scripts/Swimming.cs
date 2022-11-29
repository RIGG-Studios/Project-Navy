using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swimming : MonoBehaviour
{
    [SerializeField] private UnderwaterChecker underwaterChecker;
    
    private Player _player;

    private bool _stoppedMovement;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        bool underWater = underwaterChecker.IsUnderWater();

        if (underWater)
        {
            _player.ResetPlayerToSpawnPoint();
        }

    }
}
