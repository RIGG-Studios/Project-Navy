using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class OceanTileManager : MonoBehaviour
{
    [SerializeField] private int columnLength;
    [SerializeField] private int rowLength;
    [SerializeField] private float spacing = 1f;
    [SerializeField] private GameObject tilePrefab;

    private List<OceanTile> _tiles = new List<OceanTile>();

    private MeshFilter _meshFilter;
    private bool _setup;
    

    private void Start()
    {
        GenerateTiles();
    }
    
    public void GenerateTiles()
    {
        if (_tiles.Count > 0)
        {
            ResetTiles();
        }
        
        for (int i = 0; i < columnLength * rowLength; i++)
        {
            Vector3 pos = transform.position + new Vector3(spacing * (i % columnLength), 0, spacing * (i / columnLength));

            OceanTile tile = Instantiate(tilePrefab, pos, quaternion.identity).GetComponent<OceanTile>();
            tile.transform.SetParent(transform);
            _tiles.Add(tile);
        }
    }

    public void ResetTiles()
    {
        for (int i = 0; i < _tiles.Count; i++)
        {
            DestroyImmediate(_tiles[i].gameObject);
        }
        
        _tiles.Clear();
    }
}
