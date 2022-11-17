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

    [Header("OCEAN SETTINGS")] 
    [SerializeField] private Transform waterMesh;
    [SerializeField] private float updateDistance = 10f;

    
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

        foreach (WaveSettings.WaveDirectionData waveDirections in waveSettings.direction)
        {
         //   oceanMaterial.SetVector(waveDirections.shaderID, waveDirections.direction);
        }

        //ocean
        oceanMaterial.SetColor("_Color 01",  oceanSettings.color1);
        oceanMaterial.SetColor("_Color 02",  oceanSettings.color2);
        oceanMaterial.SetColor("_FoamColor",  oceanSettings.foamColor);
        oceanMaterial.SetFloat("_RefractionStrength", oceanSettings.refractionStrength);
        oceanMaterial.SetFloat("_DepthStrength", oceanSettings.depthStrength);
        oceanMaterial.SetFloat("_WaterOpacity", oceanSettings.waterOpacity);
        oceanMaterial.SetFloat("_WaterEdgeOpacity", oceanSettings.waterEdgeOpacity);

    }

    public float GetWaterHeightAtPosition(Vector3 pos)
    {
        float height = 0.0f;
        
        foreach (WaveSettings.WaveDirectionData waveDirections in waveSettings.direction)
        {
            height += CalculateWaveHeight(pos, waveDirections.direction);
        }

        return height;
    }

    private float CalculateWaveHeight(Vector3 pos, Vector2 direction)
    {
        float waveAmp = waveSettings.amplitude * waveSettings.steepness;

        direction.Normalize();
        Vector2 dir = -1f * direction;
        dir *= waveSettings.frequency;

        float speed = waveSettings.speed * Time.time;

        float dot = Vector2.Dot(dir,new Vector2(pos.x, pos.z));
        float total = speed + dot;

        return Mathf.Cos(total) * (waveAmp * direction.y);
    }

    private void Update()
    {
        UpdateWaterMeshPosition(Camera.main, waterMesh);
    }

    public void UpdateWaterMeshPosition(Camera cam, Transform waterMeshTransform)
    {
        if (cam != null && waterMeshTransform != null)
        {
            if (!IsCanRendererForCurrentCamera()) 
                return;
            
            
            var pos = waterMeshTransform.position;
            var camPos = cam.transform.position;

            var relativeToCamPos = new Vector3(camPos.x, pos.y, camPos.z);
            
            
            if (Vector3.Distance(pos, relativeToCamPos) >= updateDistance)
            {
                waterMeshTransform.position = relativeToCamPos;
            }
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