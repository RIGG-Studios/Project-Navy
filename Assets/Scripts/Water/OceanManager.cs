using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OceanManager : MonoBehaviour
{
    public OceanTileManager TileManager => GetComponentInChildren<OceanTileManager>();
}


[CustomEditor(typeof(OceanManager))]
public class OceanManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        OceanManager oceanManager = target as OceanManager;

        if (GUILayout.Button("Update Tiles"))
        {
            oceanManager.TileManager.GenerateTiles();
        }
        
        if (GUILayout.Button("Reset Tiles"))
        {
            oceanManager.TileManager.ResetTiles();
        }
    }
}
