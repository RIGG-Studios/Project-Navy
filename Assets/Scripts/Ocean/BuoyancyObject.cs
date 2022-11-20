using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BuoyancyObject : MonoBehaviour
{
    public ModelSourceEnum VolumeSource = ModelSourceEnum.Mesh;
    public Transform OvverideCenterOfMass;

    public float intensity;
    [FormerlySerializedAs("Density")] [Range(100, 1000)] public float density = 450;
    [Range(1, 6)] public int slicesPerAxisX = 2;
    [Range(1, 6)] public int slicesPerAxisY = 2;
    [Range(1, 6)] public int slicesPerAxisZ = 2;
    public bool isConcave = false;

    [FormerlySerializedAs("VoxelsLimit")] [Range(2, 32)] public int voxelsLimit = 16;
    [FormerlySerializedAs("AngularDrag")] public float angularDrag = 0.25f;
    [FormerlySerializedAs("Drag")] public float drag = 0.25f;
    public bool DebugForces = false;

    
    private const float DAMPFER = 0.5f;
    private const float WATER_DENSITY = 1000;

    private Vector3 _localArchimedesForce;
    private List<Vector3> _voxels;
    private Vector3[] _currentForces;
    private bool _isMeshCollider;
    private List<Vector3[]> _debugForces;

    public Rigidbody rigidBody { get; private set; }
    private Collider _collider;

    float _bounceMaxSize;

    public enum ModelSourceEnum
    {
        Collider,
        Mesh
    }
    
    private void OnEnable()
    {
        _debugForces = new List<Vector3[]>(); 

        rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();


        Quaternion originalRotation = transform.rotation;
        Vector3 originalPosition = transform.position;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        var bounds = _collider.bounds;
        _bounceMaxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        _isMeshCollider = GetComponent<MeshCollider>() != null;


        if (OvverideCenterOfMass)
            rigidBody.centerOfMass = transform.InverseTransformPoint(OvverideCenterOfMass.transform.position);
        //else rigidBody.centerOfMass = new Vector3(0, -bounds.extents.y * 0f, 0) + transform.InverseTransformPoint(bounds.center);

        rigidBody.angularDrag = angularDrag;
        rigidBody.drag = drag;
        _voxels = SliceIntoVoxels(_isMeshCollider && isConcave);
        _currentForces = new Vector3[_voxels.Count];
        
        transform.rotation = originalRotation;
        transform.position = originalPosition;

        float volume = rigidBody.mass / density;

        WeldPoints(_voxels, voxelsLimit);

        float archimedesForceMagnitude = WATER_DENSITY * Mathf.Abs(Physics.gravity.y) * volume;
        _localArchimedesForce = new Vector3(0, archimedesForceMagnitude, 0) / _voxels.Count;
        Debug.Log(_localArchimedesForce);
    }
    

    Bounds GetCurrentBounds()
    {
        Bounds bounds = new Bounds();
        if (VolumeSource == ModelSourceEnum.Mesh) bounds = GetComponent<Renderer>().bounds;
        else if (VolumeSource == ModelSourceEnum.Collider)
        {
            var meshCollider = GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                bounds = meshCollider.sharedMesh.bounds;
            }
            else bounds = GetComponent<Collider>().bounds;
        }

        return bounds;
    }

    private List<Vector3> SliceIntoVoxels(bool concave)
    {
        var points = new List<Vector3>(slicesPerAxisX * slicesPerAxisY * slicesPerAxisZ);

        var bounds = GetCurrentBounds();

        if (concave)
        {
            var meshCol = GetComponent<MeshCollider>();

            var convexValue = meshCol.convex;
            meshCol.convex = false;
            
            for (int ix = 0; ix < slicesPerAxisX; ix++)
            {
                for (int iy = 0; iy < slicesPerAxisY; iy++)
                {
                    for (int iz = 0; iz < slicesPerAxisZ; iz++)
                    {
                        float x = bounds.min.x + bounds.size.x / slicesPerAxisX * (0.5f + ix);
                        float y = bounds.min.y + bounds.size.y / slicesPerAxisY * (0.5f + iy);
                        float z = bounds.min.z + bounds.size.z / slicesPerAxisZ * (0.5f + iz);

                        var p = transform.InverseTransformPoint(new Vector3(x, y, z));

                        if (PointIsInsideMeshCollider(meshCol, p))
                        {
                            points.Add(p);
                        }
                    }
                }
            }

            if (points.Count == 0)
            {
                points.Add(bounds.center);
            }

            meshCol.convex = convexValue;
        }
        else
        {
            for (int ix = 0; ix < slicesPerAxisX; ix++)
            {
                for (int iy = 0; iy < slicesPerAxisY; iy++)
                {
                    for (int iz = 0; iz < slicesPerAxisZ; iz++)
                    {
                        float x = bounds.min.x + bounds.size.x / slicesPerAxisX * (0.5f + ix);
                        float y = bounds.min.y + bounds.size.y / slicesPerAxisY * (0.5f + iy);
                        float z = bounds.min.z + bounds.size.z / slicesPerAxisZ * (0.5f + iz);

                        var p = transform.InverseTransformPoint(new Vector3(x, y, z));

                        points.Add(p);
                    }
                }
            }
        }

        return points;
    }

    private bool PointIsInsideMeshCollider(Collider c, Vector3 p)
    {
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        foreach (var ray in directions)
        {
            RaycastHit hit;
            if (c.Raycast(new Ray(p - ray * 1000, ray), out hit, 1000f) == false)
            {
                return false;
            }
        }

        return true;
    }


    private static void FindClosestPoints(IList<Vector3> list, out int firstIndex, out int secondIndex)
    {
        float minDistance = float.MaxValue, maxDistance = float.MinValue;
        firstIndex = 0;
        secondIndex = 1;

        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                float distance = Vector3.Distance(list[i], list[j]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    firstIndex = i;
                    secondIndex = j;
                }

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                }
            }
        }
    }


    private static void WeldPoints(IList<Vector3> list, int targetCount)
    {
        if (list.Count <= 2 || targetCount < 2)
        {
            return;
        }

        while (list.Count > targetCount)
        {
            int first, second;
            FindClosestPoints(list, out first, out second);

            var mixed = (list[first] + list[second]) * 0.5f;
            list.RemoveAt(
                second); 
            list.RemoveAt(first);
            list.Add(mixed);
        }
    }


    private void FixedUpdate()
    {
        if (DebugForces) _debugForces.Clear();

        for (int i = 0; i < _voxels.Count; i++)
        {
            Vector3 wp = transform.TransformPoint(_voxels[i]);
            float waterHeight = Ocean.Instance.GetWaterHeightAtPosition(wp);
            
            Vector3 force = Vector3.zero;

            float k = wp.y - waterHeight;
            
            if (k > 1)
            {
                k = 1f;
            }
            else if (k < 0)
            {
                k = 0;
                force =  -Physics.gravity * intensity;
            }

            _currentForces[i] = force;
            rigidBody.AddForceAtPosition(force, wp, ForceMode.Acceleration);

            if (DebugForces) _debugForces.Add(new[] { wp, force }); 

        }
    }

    private void OnDrawGizmos()
    {
        if (!DebugForces) return;

        if (_voxels == null || _debugForces == null)
        {
            return;
        }

        float gizmoSize = 0.02f * _bounceMaxSize;
        Gizmos.color = Color.yellow;

        foreach (var p in _voxels)
        {
            Gizmos.DrawCube(transform.TransformPoint(p), new Vector3(gizmoSize, gizmoSize, gizmoSize));
        }

        Gizmos.color = Color.cyan;

        foreach (var force in _debugForces)
        {
            Gizmos.DrawCube(force[0], new Vector3(gizmoSize, gizmoSize, gizmoSize));
            Gizmos.DrawRay(force[0], (force[1] / rigidBody.mass) * _bounceMaxSize * 0.25f);
        }
    }
    
}

