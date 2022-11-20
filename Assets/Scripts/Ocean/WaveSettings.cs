using System;
using UnityEngine;


[CreateAssetMenu]
public class WaveSettings : ScriptableObject
{
    [Header("WAVE SETTINGS")]
    public float amplitude = 0.35f;
    public float frequency = 0.6f;
    public float speed = 0.5f;
    public float steepness = 0.5f;
    public Vector3 direction;

    [Header("SHADER SETTINGS")]
    public string amplitudeID = "_Amplitude";
    public string frequencyID = "_Frequency";
    public string speedID = "_Speed";
    public string steepnessID = "_Steepness";
    public string directionID = "_Direction";

    private void OnValidate()
    {
        if (Ocean.Instance == null)
        {
            Ocean.Instance = FindObjectOfType<Ocean>();
        }

        if (Ocean.Instance != null)
        {
            Ocean.Instance.UpdateShaderProperties();
        }
    }
}
