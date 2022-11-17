using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlaneGenerator : MonoBehaviour
{
    public bool saveToAssets;
    public int resolution;
    public float size;


    public void CreatePlane()
    {
        Utility.GenerateOceanPlane(resolution, size, saveToAssets);
    }
}

[CustomEditor(typeof(PlaneGenerator))]
public class PlaneGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlaneGenerator planeGenerator = target as PlaneGenerator;

        planeGenerator.saveToAssets = EditorGUILayout.Toggle("Save To Assets", planeGenerator.saveToAssets);
        planeGenerator.resolution = EditorGUILayout.IntField("Mesh Resolution", planeGenerator.resolution);
        planeGenerator.size = EditorGUILayout.FloatField("Mesh Size", planeGenerator.size);

        
        if (GUILayout.Button("Generate Plane"))
        {
            planeGenerator.CreatePlane();
        }
    }
}


