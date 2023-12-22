using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class StartingFlag : MonoBehaviour
{
    [SerializeField] float width;
    [SerializeField] float height;
    [SerializeField] int numPointsX = 10;
    [SerializeField] int numPointsZ = 10;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    [SerializeField] float oscBaseAmplitude = 1;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        GeneratePlane();
    }
    private void GeneratePlane()
    {
        Mesh mesh = new Mesh();

        List<Vector3> verts = new List<Vector3>();

        float baseX = -(width/2);
        float spaceX = width / (numPointsX-1);
        float baseZ = (height/2);
        float spaceZ = -(height / (numPointsZ - 1));
        for (int i = 0; i < numPointsZ; i++)
        {
            for (int j = 0; j < numPointsX; j++)
            {
                verts.Add(new Vector3(baseX + spaceX * j, 0, baseZ + spaceZ * i));
            }
        }
        mesh.SetVertices(verts);

        List<int> triangles = new List<int>();
        for (int i = 0; i < verts.Count - numPointsX; i++)
        {
            if (i % numPointsX == numPointsX-1) continue;
            triangles.Add(i);
            triangles.Add(i+1);
            triangles.Add(i+numPointsX);

            triangles.Add(i+numPointsX);
            triangles.Add(i+1);
            triangles.Add(i+ numPointsX + 1);
        }
        mesh.SetTriangles(triangles, 0);

        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < numPointsZ; i++)
        {
            for (int j = 0; j < numPointsX; j++)
            {
                uvs.Add(new Vector2(j / (float)numPointsX, 1-(i / (float)numPointsZ)));
            }
        }
        mesh.SetUVs(0, uvs);
        meshFilter.sharedMesh = mesh;
    }

    private void Update()
    {
        var verts = meshFilter.sharedMesh.vertices;
        Vector3 vertex;
        float widthBy2;
        for (int i = 0; i < verts.Length; i++)
        {
            vertex = verts[i];
            widthBy2 = width / 2;
            vertex.y = Mathf.Sin(Time.realtimeSinceStartup + vertex.z + vertex.x) * oscBaseAmplitude * ((widthBy2 - Mathf.Abs(vertex.x)) / widthBy2);
            verts[i] = vertex;
        }
        meshFilter.sharedMesh.vertices = verts;
        meshFilter.sharedMesh.RecalculateNormals();
    }
}
