using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_Hanlder : MonoBehaviour
{
    // Link with original instance object
    public GameObject animation_instance;

    // Data for Drawing Line
    public LineRenderer lineRenderer;

    public float Thickness;
    public float Length;
    public int Resolution;

    public Transform startPoint;
    public Transform endPoint;
    public Vector3 normalVec;
    public Vector3 dirVec;

    // Manage Instances
    [SerializeField]
    private int Capacity = 100;
    public int Anim_Num = 0;

    public float[] Disp;
    public float[] Anim_Start_Times;
    public float[] Anim_Durs;
    public Animation_Instance[] Anims;

    void Start()
    {
        // Load information from StringClass
        LoadStringInfos();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = Thickness;
        lineRenderer.endWidth = Thickness;
        lineRenderer.positionCount = Resolution;
        // Initialize instances
        InitInstanceArray();
    }

    void LoadStringInfos()
    {
        StringClass a_string = GetComponent<StringClass>();
        a_string.CalculateDirAndNorm();
        // override properties
        startPoint = a_string.startPoint;
        endPoint = a_string.endPoint;
        normalVec = a_string.normalVec;
        dirVec = a_string.dirVec;
        Thickness = a_string.Thickness;
        Length = a_string.Length;
        Resolution = a_string.Resolution;
    }

    void InitInstanceArray()
    {
        Anim_Start_Times = new float[Capacity];
        Anim_Durs = new float[Capacity];
        Anims = new Animation_Instance[Capacity];
        Disp = new float[Resolution];
    }

    void Update()
    {
        InitializeDisp();
        UpdateAnimations();
        UpdateDiplacements();
        DrawString();
    }

    void InitializeDisp()
    {
        for (int i = 0; i < Resolution; i++)
        {
            Disp[i] = 0f;
        }
    }

    public void AddAnimation(Animation_Instance original, GameObject parent, float amplitude, float impactPoint, float duration)
    {
        Animation_Instance instance = Instantiate(original);
        instance.transform.parent = parent.transform;
        instance.SetAmplitude(amplitude);
        instance.SetImpactPoint(impactPoint);
        instance.SetDuration(duration);

        if (Anim_Num < Capacity)
        {
            Anim_Start_Times[Anim_Num] = Time.time;
            Anim_Durs[Anim_Num] = duration;
            Anims[Anim_Num] = instance;
            Anim_Num++;
        } else 
        {
            Debug.Log("Error::Overflow");
        }
    }

    void SwapFloatList(float[] List, int i, int j)
    {
        float temp = List[i];
        List[i] = List[j];
        List[j] = temp;
    }

    void SwapInstanceList(Animation_Instance[] List, int i, int j)
    {
        Animation_Instance temp = List[i];
        List[i] = List[j];
        List[j] = temp;
    }

    void UpdateAnimations()
    {
        for (int i = 0; i < Anim_Num; i++)   
        {
            if (Time.time - Anim_Start_Times[i] >= Anim_Durs[i])
            {
                SwapFloatList(Anim_Start_Times, i, Anim_Num-1);
                Anim_Start_Times[Anim_Num-1] = 0;
                SwapFloatList(Anim_Durs, i, Anim_Num-1);
                Anim_Durs[Anim_Num-1] = 0;
                SwapInstanceList(Anims, i, Anim_Num-1);
                Destroy(Anims[Anim_Num-1].gameObject);
                Anims[Anim_Num-1] = null;
                
                Debug.Log("Delete" + Time.time + Anim_Num);
                Anim_Num--;
            }
        }
    }

    void UpdateDiplacements()
    {
        for (int i = 0; i < Anim_Num; i++)
        {
            Anims[i].AddDisplacement();
        }
    }

    void DrawString()
    {
        Vector3 segmentPos = new Vector3(0, 0, 0);
        Vector3 waveDiff;
        for (int i = 0; i < Resolution; i++)
        {
            segmentPos += Length / Resolution * dirVec;
            waveDiff = Disp[i] * normalVec;

            lineRenderer.SetPosition(i, startPoint.localPosition + segmentPos + waveDiff);
        }
    }

}
