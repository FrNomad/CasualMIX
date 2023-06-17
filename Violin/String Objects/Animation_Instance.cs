using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_Instance : MonoBehaviour
{
    // Components
    public GameObject string_object;
    public StringClass a_string;
    public Animation_Hanlder a_handler;

    // Inherited Properties
    public float Tension;
    public float LinearDensity;
    public float Length;

    public float Thickness;
    public int Resolution;

    // Properties
    public float Amplitude = 0.2f;
    public float ImpactPoint = 0.5f;

    public float StartTime;
    public float Duration;

    void Start()
    {
        LoadStringInfos();
        StartTime = Time.time;
    }

    void LoadStringInfos()
    {
        a_string = string_object.GetComponent<StringClass>();
        a_handler = string_object.GetComponent<Animation_Hanlder>();
        // -----------------------------
        Tension = a_string.Tension;
        LinearDensity = a_string.LinearDensity;
        Length = a_string.Length;
        // -----------------------------
        Thickness = a_string.Thickness;
        Resolution = a_string.Resolution;
    }

    public void SetAmplitude(float amplitude)
    {
        Amplitude = amplitude;
    }

    public void SetImpactPoint(float impactPoint)
    {
        ImpactPoint = impactPoint;
    }

    public void SetDuration(float duration)
    {
        Duration = duration;
    }

    public void AddDisplacement()
    {
        float d, damping;
        for (int i = 0; i < a_handler.Resolution; i++)
        {
            damping = Mathf.Min(1 / Mathf.Pow(1.5f * (Time.time - StartTime), 4), 1f);
            d = (float)i / (float)Resolution;
            if (d <= ImpactPoint) {
                a_handler.Disp[i] += damping * Amplitude * Mathf.Pow(d / ImpactPoint, 0.2f) * Mathf.Sin(30f * (Time.time - StartTime + Mathf.PI / 2));
            } else {
                a_handler.Disp[i] += damping * Amplitude * Mathf.Pow((1 - d) / (1 - ImpactPoint), 0.2f) * Mathf.Sin(30f * (Time.time - StartTime + Mathf.PI / 2));
            }
        }
    }
}

