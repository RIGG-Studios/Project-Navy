using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class OceanPresets : ScriptableObject
{
    //waves
    public WavePreset waves;
    
    [Space]
    
    //color
    public Color shallowColor;
    public Color deepColor;
    public Color foamColor;

    [Space]
    
    //wave foam
    public bool waveFoam = true;
    public float extraDispersion = 0.64f;
    public float edgeHardness = 1.5f;
    public float crestSize = 2.66f;
    public float crestOffset = -1f;
    
    [Space]
    
    //foam
    public bool edgeFoam = true;
    public float foamFallOff = 2.28f;
    public float foamWidth = 7.71f;
    public float foamRemoval = 0.21f;
    public float foamBands = 5;
    
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
