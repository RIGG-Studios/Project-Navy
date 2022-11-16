using System;
using Unity.VisualScripting;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    public static Ocean Instance;

        [Header("MATERIAL SETTINGS")] 
    
    [SerializeField] private Material oceanMaterial;
    
    [Header("SHADER SETTINGS")]
    
    [SerializeField] private bool executeInEditor = true;
    [SerializeField] private OceanSettings oceanSettings;
    [SerializeField] private WaveSettings waveSettings;

    private void Awake()
    {
        Instance = this;
    }


    private void OnValidate()
    {
        UpdateShaderProperties();
    }

    public void UpdateShaderProperties()
    {
        if (oceanMaterial == null || !executeInEditor)
            return;
        
        //waves
        oceanMaterial.SetFloat("_Amplitube", waveSettings.amplitude);
        oceanMaterial.SetFloat("_Steepness", waveSettings.steepness);
        oceanMaterial.SetFloat("_Frequency", waveSettings.frequency);
        oceanMaterial.SetFloat("_Speed", waveSettings.speed);
        oceanMaterial.SetVector("_Direction", waveSettings.direction);
        
        
        Debug.Log(oceanMaterial);
        //ocean
        oceanMaterial.SetColor("_Color 01",  oceanSettings.color1);
        oceanMaterial.SetColor("_Color 02",  oceanSettings.color2);
        oceanMaterial.SetColor("_FoamColor",  oceanSettings.foamColor);
        oceanMaterial.SetFloat("_RefractionStrength", oceanSettings.refractionStrength);
        oceanMaterial.SetFloat("_DepthStrength", oceanSettings.depthStrength);
        oceanMaterial.SetFloat("_WaterOpacity", oceanSettings.waterOpacity);
        oceanMaterial.SetFloat("_WaterEdgeOpacity", oceanSettings.waterEdgeOpacity);

    }

    public float GetWaterHeightAtPosition(Vector3 pos, float time)
    {
        float waveAmp = waveSettings.amplitude * waveSettings.steepness;

        Vector2 dir = -1f * waveSettings.direction;
        dir *= waveSettings.frequency;

        float t = waveSettings.speed * time;

        float dot = Vector3.Dot(pos, dir);
        float sum = t + dot;
        float y = waveAmp * dir.y;

        float cosine = Mathf.Cos(sum);

        return cosine * y;
    }
}