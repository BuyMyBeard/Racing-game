using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ShowNormalsComponent : MonoBehaviour
{
    private Vector3[] normals;
    private Vector3[] vertices;
    private int[] triangles;
    private int nNormals;

    public bool up = false;
    // Start is called before the first frame update
    void Start()
    {
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        normals = mesh.normals;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        nNormals = normals.Length;
    }

    private void Update()
    {
        if (up)
            Start();            
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        
        Vector3 avgNormal = new Vector3();
        Vector3 avgVertex = new Vector3();
        Gizmos.color = Color.green;
        for (int i = 0; i < triangles.Length / 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                avgNormal += normals[triangles[i * 3 + j]];
                avgVertex += vertices[triangles[i * 3 + j]];
            }

            avgVertex /= 3;
            avgNormal /= 3;
            Gizmos.DrawRay(transform.localToWorldMatrix.MultiplyPoint3x4(avgVertex), transform.rotation * avgNormal.normalized);
            avgNormal = Vector3.zero;
            avgVertex = Vector3.zero;
        }

        Gizmos.color = Color.white;
        for (int i = 0; i < nNormals; ++i)
            Gizmos.DrawRay(transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]), transform.rotation * normals[i]/4);
    }
}
