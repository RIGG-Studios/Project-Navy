using System;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    public static Ocean Instance;

    [Header("MATERIAL SETTINGS")] 
    
    [SerializeField] private Material oceanMaterial;
    
    [Header("SHADER SETTINGS")]
    
    [SerializeField] private bool executeInEditor = true;
    [SerializeField] private float amplitube;
    [SerializeField] private float steepness;
    [SerializeField] private float frequency;
    [SerializeField] private float speed;
    [SerializeField] private Vector2 direction;


    private void Awake()
    {
        Instance = this;
    }

    public float GetWaterHeightAtPosition(Vector3 pos, float time)
    {
        float waveAmp = amplitube * steepness;

        Vector2 dir = -1f * direction;
        dir *= frequency;

        float t = speed * time;

        float dot = Vector3.Dot(pos, dir);
        float sum = t + dot;
        float y = waveAmp * -dir.y;

        float cosine = Mathf.Cos(sum);

        return cosine;
    }
}