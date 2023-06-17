using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringClass : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Vector3 normalVec;
    public Vector3 dirVec;
    public Vector3 upwardVec;
    public float Angle = 0f;

    public float Tension;
    public float LinearDensity;
    public float Length = 0.325f * 20f;

    public float Thickness = 0.05f;
    public int Resolution = 200;

    public float Base_Freq;
    public int Base_idx;

    public void SetStartPoint(Transform p1)
    {
        startPoint = p1;
        CalculateDirAndNorm();
    }

    public void SetEndPoinnt(Transform p2)
    {
        endPoint = p2;
        CalculateDirAndNorm();
    }

    public void CalculateDirAndNorm()
    {
        dirVec = (endPoint.localPosition - startPoint.localPosition).normalized;
        upwardVec = Quaternion.AngleAxis(Angle, dirVec) * transform.TransformPoint(Vector3.up).normalized;
        normalVec = Vector3.Cross(dirVec, upwardVec);
    }
}
