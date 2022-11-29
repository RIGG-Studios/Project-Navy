using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVitals : MonoBehaviour
{
    [SerializeField] private float maxOxygen;
    [SerializeField] private float oxygenDecayRate;
    [SerializeField] private float outOfBreathDamageRate;
    [SerializeField] private Slider oxygenSlider;
    [SerializeField] private UnderwaterChecker underwaterChecker;

    private float _oxygen;
    private bool _outOfBreath;
    private float _damageTick;

    private Player _player;

    private void Awake()
    {
        _oxygen = maxOxygen;
        oxygenSlider.minValue = 0;
        oxygenSlider.maxValue = maxOxygen;
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        bool underWater = underwaterChecker.IsUnderWater();
        
        if (underWater)
        {
            oxygenSlider.gameObject.SetActive(true);
            if(_oxygen > 0) _oxygen -= Time.deltaTime * oxygenDecayRate;

            if (_oxygen <= 0)
            {
                _outOfBreath = true;
            }
        }
        else
        {
            oxygenSlider.gameObject.SetActive(false);
            _oxygen += Time.deltaTime * oxygenDecayRate;
            _outOfBreath = false;
        }

        oxygenSlider.value = _oxygen;

        if (_outOfBreath)
        {
            _damageTick += Time.deltaTime;

            if (_damageTick > outOfBreathDamageRate)
            {
                _player.Damage(PhotonNetwork.LocalPlayer.ActorNumber,10f);
                _damageTick = 0.0f;
            }
        }
    }
}
