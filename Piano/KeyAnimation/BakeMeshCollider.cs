using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeMeshCollider : MonoBehaviour
{
    MeshFilter mesh_filter;
    MeshCollider mesh_collider;

    void Awake()
    {
        mesh_filter = GetComponent<MeshFilter>();
        mesh_collider = gameObject.AddComponent<MeshCollider>();
        mesh_collider.sharedMesh = mesh_filter.mesh;
    }
}
