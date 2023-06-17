using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AddMeshCollider : MonoBehaviour
{
    public LineRenderer Line;

    void Start()
    {
    }

    void Update()
    {
        GenerateMeshCollider();
    }

    public void GenerateMeshCollider()
    {
        Line = GetComponent<LineRenderer>();
        MeshCollider collider = GetComponent<MeshCollider>();

        if (collider == null)
        {
            collider = gameObject.AddComponent<MeshCollider>();
        }

        Mesh mesh = new Mesh();
        Line.BakeMesh(mesh);
        collider.sharedMesh = mesh;
    }
}
