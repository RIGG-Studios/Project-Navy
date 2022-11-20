using UnityEngine;

public static class MeshGenerator
{
    public static Mesh GeneratePlane(int meshResolution, float scale, bool useXZplane = true)
    {
        var vertices = new Vector3[(meshResolution + 1) * (meshResolution + 1)];
        var uv = new Vector2[vertices.Length];
        var triangles = new int[meshResolution * meshResolution * 6];

        for (int i = 0, y = 0; y <= meshResolution; y++)
        for (var x = 0; x <= meshResolution; x++, i++)
        {
            if (useXZplane) vertices[i] = new Vector3(x * scale / meshResolution - 0.5f * scale, 0, y * scale / meshResolution - 0.5f * scale);
            else vertices[i] = new Vector3(x * scale / meshResolution - 0.5f * scale, y * scale / meshResolution - 0.5f * scale, 0);
            uv[i] = new Vector2(x * scale / meshResolution, y * scale / meshResolution);
        }

        for (int ti = 0, vi = 0, y = 0; y < meshResolution; y++, vi++)
        for (var x = 0; x < meshResolution; x++, ti += 6, vi++)
        {
            triangles[ti] = vi;
            triangles[ti + 3] = triangles[ti + 2] = vi + 1;
            triangles[ti + 4] = triangles[ti + 1] = vi + meshResolution + 1;
            triangles[ti + 5] = vi + meshResolution + 2;
        }

        Mesh mesh = new Mesh();
        
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        return mesh;
    }
}
