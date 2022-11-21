using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OceanManager : MonoBehaviour
{
    public OceanTileManager TileManager => GetComponentInChildren<OceanTileManager>();
}
