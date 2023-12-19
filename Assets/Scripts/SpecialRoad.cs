using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SpecialRoad : MonoBehaviour
{
    [SerializeField] float width;
    [SerializeField] float height;
    [SerializeField] int xSubdivisions = 2;
    [SerializeField] int ySubdivisions = 2;
    MeshCollider meshCollider;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void GeneratePlane()
    {
        Vector3[] verts = new Vector3[(xSubdivisions + 2) * (ySubdivisions + 2)];
        for (int i = 0; i < xSubdivisions + 2; i++)
        {
            for (int j = 0; j < ySubdivisions + 2; j++)
            {

            }
        }
    }
}
