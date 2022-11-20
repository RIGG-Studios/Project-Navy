using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LODGroup))]
public class OceanTile : MonoBehaviour
{
    public Vector3 GetWorldPosition(Vector3 localCoordinates)
    {
        return transform.TransformPoint(localCoordinates);
    }
}
