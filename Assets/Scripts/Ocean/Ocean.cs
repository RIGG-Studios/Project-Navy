using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    public static Ocean Instance;

    [Header("MATERIAL SETTINGS")] 
    
    [SerializeField] private Material oceanMaterial;

    [Header("SHADER SETTINGS")]
        
    [SerializeField] private bool executeInEditor = true;
    [SerializeField] private bool blendBetweenPresets = true;
    [SerializeField] private OceanPresets oceanPreset;

    [Header("OCEAN SETTINGS")] 
    [SerializeField] private Transform waterMesh;
    [SerializeField] private float updateDistance = 10f;
    
    public readonly string ShallowWaterColorID = "_Color01";
    public readonly string DeepWaterColorID = "_Color02";
    public readonly string FoamColorID = "_FoamColor";

    public readonly string ExtraDispersionID = "_ExtraExpersion";
    public readonly string FoamEdgeHardnessID = "_FoamEdgeHardness";
    public readonly string CrestSizeID = "_CrestSize";
    public readonly string CrestOffsetID = "_CrestOffset";

    public readonly string FoamFalloffID = "_FoamFalloff";
    public readonly string FoamWidthID = "_FoamWidth";
    public readonly string FoamRemovalID = "_FoamRemoval";
    public readonly string FoamBandsID = "_FoamBands";


    private OceanPresets _oceanPreset;
    
    private void Awake()
    {
        Instance = this;
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
        float waveAmp = waveSettings.amplitude * waveSettings.steepness;

        Vector3 direction = waveSettings.direction.normalized;
        Vector3 dir = -1f * direction;
        dir *= waveSettings.frequency;

        float speed = waveSettings.speed * Time.time;

        float dot = Vector3.Dot(dir,pos);
        float total = speed + dot;

        return Mathf.Cos(total) * (waveAmp * direction.y);
    }

    private void Update()
    {
        UpdateWaterMeshPosition(Camera.main, waterMesh);
    }

    public void UpdateWaterMeshPosition(Camera cam, Transform waterMeshTransform)
    {
        if (cam == null || waterMeshTransform == null)
            return;
        
        if (!IsCanRendererForCurrentCamera()) 
            return;
            
        Vector3 pos = waterMeshTransform.position;
        Vector3 camPos = cam.transform.position;

        Vector3 relativeToCamPos = new Vector3(camPos.x, pos.y, camPos.z);
        
        if (Vector3.Distance(pos, relativeToCamPos) >= updateDistance)
        {
            waterMeshTransform.position = relativeToCamPos;
        }
    }

    bool IsCanRendererForCurrentCamera()
    {
        if (Application.isPlaying)
        {
            return Camera.main.cameraType == CameraType.Game;
        }
        else
        {
            return Camera.main.cameraType == CameraType.SceneView;
        }
    }
}