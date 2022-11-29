using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    public static Ocean Instance;

    [Header("MATERIAL SETTINGS")] 
    
    [SerializeField] private Material oceanMaterial;

    [Header("SHADER SETTINGS")]
        
    [SerializeField] private bool executeInEditor = true;
    [SerializeField] private OceanPresets oceanPreset;
    
    
    private const string ShallowWaterColorID = "_Color01";
    private const string DeepWaterColorID = "_Color02";
    private const string FoamColorID = "_FoamColor";
    private const string ExtraDispersionID = "_ExtraExpersion";
    private const string FoamEdgeHardnessID = "_FoamEdgeHardness";
    private const string CrestSizeID = "_CrestSize";
    private const string CrestOffsetID = "_CrestOffset";
    private const string FoamFalloffID = "_FoamFalloff";
    private const string FoamWidthID = "_FoamWidth";
    private const string FoamRemovalID = "_FoamRemoval";
    private const string FoamBandsID = "_FoamBands";
    
    private OceanPresets _oceanPreset;

    private float _time;
    
    private void Awake()
    {
        Instance = this;
        PhotonNetwork.SendRate = 30;
    }

    public Material GetOceanMat()
    {
        return oceanMaterial;
    }


    private void OnValidate()
    {
        if (!executeInEditor)
            return;

        UpdateShaderProperties();
    }

    public void UpdateShaderProperties()
    {
        if (oceanMaterial == null)
            return;


        //waves
        foreach (WaveSettings wave in oceanPreset.waves.waves)
        {
            oceanMaterial.SetFloat(wave.amplitudeID, wave.amplitude);
            oceanMaterial.SetFloat(wave.steepnessID, wave.steepness);
            oceanMaterial.SetFloat(wave.frequencyID, wave.frequency);
            oceanMaterial.SetFloat(wave.speedID, wave.speed);
            oceanMaterial.SetVector(wave.directionID, wave.direction);
        }
        
        //colors
        oceanMaterial.SetColor(ShallowWaterColorID,  oceanPreset.shallowColor);
        oceanMaterial.SetColor(DeepWaterColorID,  oceanPreset.deepColor);
        oceanMaterial.SetColor(FoamColorID,  oceanPreset.foamColor);
        
        //wave foam
        oceanMaterial.SetFloat(ExtraDispersionID, oceanPreset.extraDispersion);
        oceanMaterial.SetFloat(FoamEdgeHardnessID, oceanPreset.edgeHardness);
        oceanMaterial.SetFloat(CrestSizeID, oceanPreset.crestSize);
        oceanMaterial.SetFloat(CrestOffsetID, oceanPreset.crestOffset);
        
        //edge foam
        oceanMaterial.SetFloat(FoamFalloffID, oceanPreset.foamFallOff);
        oceanMaterial.SetFloat(FoamWidthID, oceanPreset.foamWidth);
        oceanMaterial.SetFloat(FoamRemovalID, oceanPreset.foamRemoval);
        oceanMaterial.SetFloat(FoamBandsID, oceanPreset.foamBands);
    }

    private void Update()
    {
        _time = Time.time;
        oceanMaterial.SetFloat("_GameTime", _time);
    }

    public float GetWaterHeightAtPosition(Vector3 pos)
    {
        float height = 0.0f;

        foreach (WaveSettings wave in oceanPreset.waves.waves)
        {
            height += CalculateWaveHeight(pos, wave);
        }

        return height;
    }

    private float CalculateWaveHeight(Vector3 pos, WaveSettings waveSettings)
    {
        Vector2 dir = waveSettings.direction.normalized;
        Vector2 negatedDir = -1 * dir;

        float dot = Vector2.Dot(negatedDir * waveSettings.frequency, pos);
        float time = waveSettings.speed * _time;
        float total = dot + time;

        float waveAmp = waveSettings.amplitude * waveSettings.steepness;
        float n = waveAmp * negatedDir.y;
        return Mathf.Cos(total) * n;
    }
    
}