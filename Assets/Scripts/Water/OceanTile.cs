using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanTile : MonoBehaviour
{
    private Renderer _renderer;
    

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }
    

    public void UpdateMaterial(Material material)
    {
        _renderer.material = material;
    }
}
