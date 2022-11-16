using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu()]
public class OceanSettings : ScriptableObject
{
    public Color color1;
    public Color color2;
    public Color foamColor;
    public float refractionStrength = 0.15f;
    public float depthStrength = 1.04f;
    [Range(0, 1)] public float waterOpacity = 0.89f;
    [Range(0, 1)] public float waterEdgeOpacity = 0.55f;

    
    private void OnValidate()
    {
        if (Ocean.Instance == null)
        {
            Ocean.Instance = FindObjectOfType<Ocean>();
        }
        Ocean.Instance.UpdateShaderProperties();
    }
}
