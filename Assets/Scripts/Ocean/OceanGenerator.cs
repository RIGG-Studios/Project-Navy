using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanGenerator : MonoBehaviour
{
    [System.Flags]
    enum Seams
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,
        All = Left | Right | Top | Bottom
    }; 
    
    [SerializeField] private float lengthScale = 10;
    [SerializeField] private int vertexDensity = 30;
    [SerializeField] private int clipLevels = 8;
    [SerializeField] private float skirtSize = 50;

    private Camera _camera;

    private List<Element> _rings = new List<Element>();
    private List<Element> _trims = new List<Element>();

    private Element _center;
    private Element _skirt;
    private Quaternion[] _trimRotations;
    private int _previousVertexDensity;
    private float _previousSkirtSize;


    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
        
        _trimRotations = new Quaternion[]
        {
            Quaternion.AngleAxis(180, Vector3.up),
            Quaternion.AngleAxis(90, Vector3.up),
            Quaternion.AngleAxis(270, Vector3.up),
            Quaternion.identity,
        };
    }
    
    private void Update()
    {
        if (_rings.Count != clipLevels || _trims.Count != clipLevels
                                      || _previousVertexDensity != vertexDensity || !Mathf.Approximately(_previousSkirtSize, skirtSize))
        {
            GenerateMeshes();
            _previousVertexDensity = vertexDensity;
            _previousSkirtSize = skirtSize;
        }

        UpdatePositions();
    }


    private void OnValidate()
    {
        /*/
        if (_camera == null)
        {
            _camera = Camera.main;
            Debug.Log(_camera);
        }
        
        _trimRotations = new Quaternion[]
        {
            Quaternion.AngleAxis(180, Vector3.up),
            Quaternion.AngleAxis(90, Vector3.up),
            Quaternion.AngleAxis(270, Vector3.up),
            Quaternion.identity,
        };
        
        if (_rings.Count != clipLevels || _trims.Count != clipLevels
                                       || _previousVertexDensity != vertexDensity || !Mathf.Approximately(_previousSkirtSize, skirtSize))
        {
            GenerateMeshes();
            _previousVertexDensity = vertexDensity;
            _previousSkirtSize = skirtSize;
        }

        UpdatePositions();
        /*/
    }

    private void UpdatePositions()
    {
        int k = GridSize();
        int activeLevels = ActiveLodlevels();

        float scale = ClipLevelScale(-1, activeLevels);
        Vector3 previousSnappedPosition = Snap(_camera.transform.position, scale * 2);
        _center.Transform.position = previousSnappedPosition + OffsetFromCenter(-1, activeLevels);
        _center.Transform.localScale = new Vector3(scale, 1, scale);

        for (int i = 0; i < clipLevels; i++)
        {
            _rings[i].Transform.gameObject.SetActive(i < activeLevels);
            _trims[i].Transform.gameObject.SetActive(i < activeLevels);
            if (i >= activeLevels) continue;

            scale = ClipLevelScale(i, activeLevels);
            Vector3 centerOffset = OffsetFromCenter(i, activeLevels);
            Vector3 snappedPosition = Snap(_camera.transform.position, scale * 2);

            Vector3 trimPosition = centerOffset + snappedPosition + scale * (k - 1) / 2 * new Vector3(1, 0, 1);
            int shiftX = previousSnappedPosition.x - snappedPosition.x < float.Epsilon ? 1 : 0;
            int shiftZ = previousSnappedPosition.z - snappedPosition.z < float.Epsilon ? 1 : 0;
            trimPosition += shiftX * (k + 1) * scale * Vector3.right;
            trimPosition += shiftZ * (k + 1) * scale * Vector3.forward;
            _trims[i].Transform.position = trimPosition;
            _trims[i].Transform.rotation = _trimRotations[shiftX + 2 * shiftZ];
            _trims[i].Transform.localScale = new Vector3(scale, 1, scale);

            _rings[i].Transform.position = snappedPosition + centerOffset;
            _rings[i].Transform.localScale = new Vector3(scale, 1, scale);
            previousSnappedPosition = snappedPosition;
        }

        scale = lengthScale * 2 * Mathf.Pow(2, clipLevels);
        _skirt.Transform.position = new Vector3(-1, 0, -1) * scale * (skirtSize + 0.5f - 0.5f / GridSize()) + previousSnappedPosition;
        _skirt.Transform.localScale = new Vector3(scale, 1, scale);
    }
    
    private void GenerateMeshes()
    {
        foreach (var child in gameObject.GetComponentsInChildren<Transform>())
        {
            if (child != transform)
                DestroyImmediate(child.gameObject);
        }
        
        _rings.Clear();
        _trims.Clear();

        int k = GridSize();
        _center = InstantiateElement("Center", CreatePlaneMesh(2 * k, 2 * k, 1, Seams.All));
        Mesh ring = CreateRingMesh(k, 1);
        Mesh trim = CreateTrimMesh(k, 1);
        for (int i = 0; i < clipLevels; i++)
        {
            _rings.Add(InstantiateElement("Ring " + i, ring));
            _trims.Add(InstantiateElement("Trim " + i, trim));
        }
        
        _skirt = InstantiateElement("Skirt", CreateSkirtMesh(k, skirtSize));
    }
    
    Vector3 Snap(Vector3 coords, float scale)
    {
        if (coords.x >= 0)
            coords.x = Mathf.Floor(coords.x / scale) * scale;
        else
            coords.x = Mathf.Ceil((coords.x - scale + 1) / scale) * scale;

        if (coords.z < 0)
            coords.z = Mathf.Floor(coords.z / scale) * scale;
        else
            coords.z = Mathf.Ceil((coords.z - scale + 1) / scale) * scale;

        coords.y = 0;
        return coords;
    }
    
    int ActiveLodlevels()
    {
        return clipLevels - Mathf.Clamp((int)Mathf.Log((1.7f * Mathf.Abs(_camera.transform.position.y) + 1) / lengthScale, 2), 0, clipLevels);
    }

    float ClipLevelScale(int level, int activeLevels)
    {
        return lengthScale / GridSize() * Mathf.Pow(2, clipLevels - activeLevels + level + 1);
    }

    Vector3 OffsetFromCenter(int level, int activeLevels)
    {
        return (Mathf.Pow(2, clipLevels) + GeometricProgressionSum(2, 2, clipLevels - activeLevels + level + 1, clipLevels - 1))
            * lengthScale / GridSize() * (GridSize() - 1) / 2 * new Vector3(-1, 0, -1);
    }

    float GeometricProgressionSum(float b0, float q, int n1, int n2)
    {
        return b0 / (1 - q) * (Mathf.Pow(q, n2) - Mathf.Pow(q, n1));
    }

    int GridSize()
    {
        return 4 * vertexDensity + 1;
    }
    
     Element InstantiateElement(string name, Mesh mesh)
    {
        GameObject go = new GameObject();
        go.name = name;
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = true;
        meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.Camera;
        meshRenderer.material = Ocean.Instance.GetOceanMat();
        meshRenderer.allowOcclusionWhenDynamic = false;
        return new Element(go.transform, meshRenderer);
    }

    Mesh CreateSkirtMesh(int k, float outerBorderScale)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Clipmap skirt";
        CombineInstance[] combine = new CombineInstance[8];

        Mesh quad = CreatePlaneMesh(1, 1, 1);
        Mesh hStrip = CreatePlaneMesh(k, 1, 1);
        Mesh vStrip = CreatePlaneMesh(1, k, 1);


        Vector3 cornerQuadScale = new Vector3(outerBorderScale, 1, outerBorderScale);
        Vector3 midQuadScaleVert = new Vector3(1f / k, 1, outerBorderScale);
        Vector3 midQuadScaleHor = new Vector3(outerBorderScale, 1, 1f / k);

        combine[0].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, cornerQuadScale);
        combine[0].mesh = quad;

        combine[1].transform = Matrix4x4.TRS(Vector3.right * outerBorderScale, Quaternion.identity, midQuadScaleVert);
        combine[1].mesh = hStrip;

        combine[2].transform = Matrix4x4.TRS(Vector3.right * (outerBorderScale + 1), Quaternion.identity, cornerQuadScale);
        combine[2].mesh = quad;

        combine[3].transform = Matrix4x4.TRS(Vector3.forward * outerBorderScale, Quaternion.identity, midQuadScaleHor);
        combine[3].mesh = vStrip;

        combine[4].transform = Matrix4x4.TRS(Vector3.right * (outerBorderScale + 1)
            + Vector3.forward * outerBorderScale, Quaternion.identity, midQuadScaleHor);
        combine[4].mesh = vStrip;

        combine[5].transform = Matrix4x4.TRS(Vector3.forward * (outerBorderScale + 1), Quaternion.identity, cornerQuadScale);
        combine[5].mesh = quad;

        combine[6].transform = Matrix4x4.TRS(Vector3.right * outerBorderScale
            + Vector3.forward * (outerBorderScale + 1), Quaternion.identity, midQuadScaleVert);
        combine[6].mesh = hStrip;

        combine[7].transform = Matrix4x4.TRS(Vector3.right * (outerBorderScale + 1)
            + Vector3.forward * (outerBorderScale + 1), Quaternion.identity, cornerQuadScale);
        combine[7].mesh = quad;
        mesh.CombineMeshes(combine, true);
        return mesh;
    }

    Mesh CreateTrimMesh(int k, float lengthScale)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Clipmap trim";
        CombineInstance[] combine = new CombineInstance[2];

        combine[0].mesh = CreatePlaneMesh(k + 1, 1, lengthScale, Seams.None, 1);
        combine[0].transform = Matrix4x4.TRS(new Vector3(-k - 1, 0, -1) * lengthScale, Quaternion.identity, Vector3.one);

        combine[1].mesh = CreatePlaneMesh(1, k, lengthScale, Seams.None, 1);
        combine[1].transform = Matrix4x4.TRS(new Vector3(-1, 0, -k - 1) * lengthScale, Quaternion.identity, Vector3.one);

        mesh.CombineMeshes(combine, true);
        return mesh;
    }

    Mesh CreateRingMesh(int k, float lengthScale)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Clipmap ring";
        if ((2 * k + 1) * (2 * k + 1) >= 256 * 256)
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        CombineInstance[] combine = new CombineInstance[4];

        combine[0].mesh = CreatePlaneMesh(2 * k, (k - 1) / 2, lengthScale, Seams.Bottom | Seams.Right | Seams.Left);
        combine[0].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

        combine[1].mesh = CreatePlaneMesh(2 * k, (k - 1) / 2, lengthScale, Seams.Top | Seams.Right | Seams.Left);
        combine[1].transform = Matrix4x4.TRS(new Vector3(0, 0, k + 1 + (k - 1) / 2) * lengthScale, Quaternion.identity, Vector3.one);

        combine[2].mesh = CreatePlaneMesh((k - 1) / 2, k + 1, lengthScale, Seams.Left);
        combine[2].transform = Matrix4x4.TRS(new Vector3(0, 0, (k - 1) / 2) * lengthScale, Quaternion.identity, Vector3.one);

        combine[3].mesh = CreatePlaneMesh((k - 1) / 2, k + 1, lengthScale, Seams.Right);
        combine[3].transform = Matrix4x4.TRS(new Vector3(k + 1 + (k - 1) / 2, 0, (k - 1) / 2) * lengthScale, Quaternion.identity, Vector3.one);

        mesh.CombineMeshes(combine, true);
        return mesh;
    }

    Mesh CreatePlaneMesh(int width, int height, float lengthScale, Seams seams = Seams.None, int trianglesShift = 0)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Clipmap plane";
        if ((width + 1) * (height + 1) >= 256 * 256)
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
        int[] triangles = new int[width * height * 2 * 3];
        Vector3[] normals = new Vector3[(width + 1) * (height + 1)];

        for (int i = 0; i < height + 1; i++)
        {
            for (int j = 0; j < width + 1; j++)
            {
                int x = j;
                int z = i;

                if ((i == 0 && seams.HasFlag(Seams.Bottom)) || (i == height && seams.HasFlag(Seams.Top)))
                    x = x / 2 * 2;
                if ((j == 0 && seams.HasFlag(Seams.Left)) || (j == width && seams.HasFlag(Seams.Right)))
                    z = z / 2 * 2;

                vertices[j + i * (width + 1)] = new Vector3(x, 0, z) * lengthScale;
                normals[j + i * (width + 1)] = Vector3.up;
            }
        }

        int tris = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int k = j + i * (width + 1);
                if ((i + j + trianglesShift) % 2 == 0)
                {
                    triangles[tris++] = k;
                    triangles[tris++] = k + width + 1;
                    triangles[tris++] = k + width + 2;

                    triangles[tris++] = k;
                    triangles[tris++] = k + width + 2;
                    triangles[tris++] = k + 1;
                }
                else
                {
                    triangles[tris++] = k;
                    triangles[tris++] = k + width + 1;
                    triangles[tris++] = k + 1;

                    triangles[tris++] = k + 1;
                    triangles[tris++] = k + width + 1;
                    triangles[tris++] = k + width + 2;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        return mesh;
    }

    private class Element
    {
        public Transform Transform;
        public MeshRenderer MeshRenderer;

        public Element(Transform transform, MeshRenderer meshRenderer)
        {
            Transform = transform;
            MeshRenderer = meshRenderer;
        }
    }
}
