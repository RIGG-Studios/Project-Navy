using System;
using UnityEngine;


[CreateAssetMenu]
public class WaveSettings : ScriptableObject
{
    public float amplitude = 0.35f;
    public float frequency = 0.6f;
    public float speed = 0.5f;
    public float steepness = 0.5f;
    public Vector2 direction;


    private void OnValidate()
    {
        if (Ocean.Instance == null)
        {
            Ocean.Instance = FindObjectOfType<Ocean>();
        }
        Ocean.Instance.UpdateShaderProperties();
    }
}
