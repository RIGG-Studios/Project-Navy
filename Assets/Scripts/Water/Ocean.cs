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
        oceanMaterial.SetVector("_Direction", waveSettings.direction);
        
        
        //ocean
        oceanMaterial.SetColor("_Color 01",  oceanSettings.color1);
        oceanMaterial.SetColor("_Color 02",  oceanSettings.color2);
        oceanMaterial.SetColor("_FoamColor",  oceanSettings.foamColor);
        oceanMaterial.SetFloat("_RefractionStrength", oceanSettings.refractionStrength);
        oceanMaterial.SetFloat("_DepthStrength", oceanSettings.depthStrength);
        oceanMaterial.SetFloat("_WaterOpacity", oceanSettings.waterOpacity);
        oceanMaterial.SetFloat("_WaterEdgeOpacity", oceanSettings.waterEdgeOpacity);

    }

    public Vector3 GetWaterHeightAtPosition(Vector3 pos, float time)
    {
        float waveAmp = waveSettings.amplitude * waveSettings.steepness;

        Vector2 dir = -1f * waveSettings.direction;
        dir *= waveSettings.frequency;

        float t = waveSettings.speed * time;

        float dot = Vector2.Dot(pos, dir);
        float sum = t + dot;
        float height = waveAmp * dir.y;

        float cosine = Mathf.Cos(sum);
        float y =  cosine * height;

        float x = (waveAmp * dir.x) * cosine;
        float z = Mathf.Sin(sum) * waveSettings.amplitude;

        return new Vector3(x, y, z);
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