using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Draw_Base_Line : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Detache_Handler detache_handler;
    public GameObject selected_string;
    private StringClass a_string;
    public Vector3 dirVec;
    public Vector3 normalVec;
    public Vector3 intersect;

    //public int Resolution;
    public float Length;
    public float Width;

    //attributes for mouse interactions
    public float Depth;
    private float mouse_depth;
    private float mouse_position;

    private Mesh mesh;
    private MeshRenderer mesh_renderer;
    private MeshCollider mesh_collider;
    public Vector3[] vertices;
    private int[] triangles;

    public Material m_Material;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = Width;
        lineRenderer.endWidth = Width;
        lineRenderer.positionCount = 2;

        detache_handler = GameObject.FindWithTag("MainCamera").GetComponent<Detache_Handler>();

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
            
        //mesh_renderer = gameObject.AddComponent<MeshRenderer>();
        //mesh_renderer.materials[0] = m_Material;

        mesh_collider = gameObject.AddComponent<MeshCollider>();
        mesh_collider.sharedMesh = mesh;
    }

    void Update()
    {
        if (a_string != null)
        {
            ShowBaseLine();
        }
    }

    public bool isShown()
    {
        return a_string != null && vertices.Length != 0;
    }

    public void HideBaseLine()
    {
        lineRenderer.positionCount = 0;
        HideMesh();
    }

    public void ShowBaseLine()
    {
        // update mutable linked data
        lineRenderer.positionCount = 2;
        float offset_stirng = detache_handler.offset_stirng;
        float offset_bow = detache_handler.offset_bow;
        dirVec = a_string.normalVec;
        normalVec = a_string.upwardVec;
        intersect = a_string.startPoint.position + offset_stirng * a_string.Length * a_string.dirVec;
        Vector3 p1 = intersect - (1f - offset_bow) * Length * dirVec;
        Vector3 p2 = intersect + offset_bow * Length * dirVec;
        lineRenderer.SetPosition(0, p1);
        lineRenderer.SetPosition(1, p2);

        ShowMesh(new Vector3[]{p1, p1 + Depth * normalVec, p2 + Depth * normalVec, p2}, new int[]{0, 1, 2, 0, 2, 3});
    }

    public void UpdateString()
    {
        GameObject Selected_String = selected_string.GetComponent<Load_Selected_String>().Selected_String;
        if (Selected_String != null)
        {
            a_string = Selected_String.GetComponent<StringClass>();
            a_string.CalculateDirAndNorm();
            dirVec = a_string.normalVec;
        } else
        {
            a_string = null;
        }
    }

    public void ShowMesh(Vector3[] v, int[] t)
    {
        vertices = v;
        triangles = new int[t.Length * 2];
        for (int i = 0; i < t.Length / 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                triangles[6 * i + j] = t[3 * i + j];
            }
            for (int k = 0; k < 3; k++)
            {
                triangles[6 * i + k + 3] = t[3 * i + 2 - k];
            }
        }
        //Debug.Log(triangles[0] + "," + triangles[1] + "," + triangles[2] + "," + triangles[3] + "," + triangles[4] + "," + triangles[5] + "," + triangles[6] + "," + triangles[7] + "," + triangles[8] + "," + triangles[9] + "," + triangles[10] + "," + triangles[11]);
        UpdateMesh();
    }

    public void HideMesh()
    {
        vertices = new Vector3[]{};
        triangles = new int[]{};
        UpdateMesh();
    }

    public void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        if (isShown())
        {
            mesh_collider.sharedMesh = mesh;
        }
        //Debug.Log("Update Mesh");
    }
}
