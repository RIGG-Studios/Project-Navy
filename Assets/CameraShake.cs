using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private ShakeSettings[] shakeSettings;
        
    [System.Serializable]
    public class ShakeSettings
    {
        public string id;
        public AnimationCurve curve;
        public float duration;
    }
    

    public void ShakeCamera(string id)
    {
        ShakeSettings shakeSettings = GetShakeSetting(id);

        if (shakeSettings == null)
        {
            return;
        }

        StartCoroutine(Shake(shakeSettings));
    }

    private IEnumerator Shake(ShakeSettings shakeSettings)
    {
        Vector3 startPos = transform.localPosition;
        float elapsedTime = 0.0f;

        while (elapsedTime < shakeSettings.duration)
        {
            elapsedTime += Time.deltaTime;

            float strength = shakeSettings.curve.Evaluate(elapsedTime / shakeSettings.duration);
            transform.localPosition = startPos + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.localPosition = startPos;
    }

    public ShakeSettings GetShakeSetting(string id)
    {
        for (int i = 0; i < shakeSettings.Length; i++)
        {
            if (id == shakeSettings[i].id)
            {
                return shakeSettings[i];
            }
        }

        return null;
    }
}
