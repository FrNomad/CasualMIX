using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;

public class Detache_Handler : MonoBehaviour
{
    public float offset_stirng = 0.5f;
    public float offset_bow = 0.5f;

    public float sensitivity_string = 0.1f;
    public float sensitivity_bow = 0.1f;

    [SerializeField] private float u, v;
    [SerializeField] private float du, dv;
    private float pdu, pdv;

    [SerializeField] private float octave, finger;

    public GameObject selected_string;
    public GameObject base_line;
    public GameObject bow_object;

    private Ray ray;
    private RaycastHit hit;

    private Pitch_Handler pitch_handler;

    void Awake()
    {
        pitch_handler = GetComponent<Pitch_Handler>();
    }

    public void Run()
    {
        Draw_Base_Line draw_base_line = base_line.GetComponent<Draw_Base_Line>();

        // If left mouse button is clicked, update the Select String GameObject
        if (Input.GetMouseButtonDown(0))
        {
            GameObject nearest_object = Util_Camera.FindClosestStringInCircleRange(Input.mousePosition, 30f);
            if (nearest_object != null)
            {
                selected_string.GetComponent<Load_Selected_String>().Selected_String = nearest_object;
                draw_base_line.UpdateString();
                draw_base_line.ShowBaseLine();
                offset_stirng = Util_String.GetKofString(nearest_object, Input.mousePosition);

                GameObject selected = selected_string.GetComponent<Load_Selected_String>().Selected_String;
                GameObject sound_object = selected.transform.parent.Find("StringSound").gameObject;
                Audio_Handler audio_handler_script = selected.GetComponent<Audio_Handler>();

                audio_handler_script.PlayAudio(0.1f);
            }
        }

        // Using mouse buttons and scroll, user can control the position of bow
        if (Input.GetMouseButton(1))
        {
            offset_stirng += sensitivity_string * Input.mouseScrollDelta.y;
            offset_stirng = Mathf.Clamp(offset_stirng, 0f, 1f);
        } else 
        {
            offset_bow += sensitivity_bow * Input.mouseScrollDelta.y;
            offset_bow = Mathf.Clamp(offset_bow, 0f, 1f);
        }
        
        // If user rotates its view, then the playing mode will be quit
        if (Input.GetMouseButtonDown(1))
        {
            if (selected_string != null)
            {
                selected_string.GetComponent<Load_Selected_String>().Selected_String = null;
                draw_base_line.UpdateString();
                draw_base_line.HideBaseLine();
            }
        }
    }

    public void Interact()
    {
        Draw_Base_Line draw_base_line = base_line.GetComponent<Draw_Base_Line>();
        if (draw_base_line.isShown())
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Debug.Log("isShown");
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("RayCast");
                if (hit.collider != null)
                {
                    //Debug.Log("Collide");
                    if (hit.collider.gameObject.name == "Baseline")
                    {
                        //Debug.Log("Baseline");
                        Vector3[] vertices = draw_base_line.vertices;
                        Vector3 p = hit.point;
                        pdu = du; pdv = dv;
                        du = u; dv = v;
                        u = Vector3.Project(p - vertices[0], vertices[3] - vertices[0]).magnitude / draw_base_line.Length;
                        v = Vector3.Project(p - vertices[0], vertices[1] - vertices[0]).magnitude / draw_base_line.Depth;
                        du = Mathf.Lerp(pdu, Mathf.Abs(u - du), 0.1f); dv = Mathf.Lerp(pdv, Mathf.Abs(u - dv), 0.1f);
                        //du = u - du; dv = v - dv;

                        if (!(u >= 0.05f && u <= 0.95f && v >= 0.05f && v <= 0.95f))
                        {
                            //selected_string.GetComponent<Load_Selected_String>().Selected_String = null;
                            //draw_base_line.UpdateString();
                            //draw_base_line.HideBaseLine();
                            //du = 0; dv = 0;

                            u = Mathf.Clamp(u, 0.05f, 0.95f); v = Mathf.Clamp(v, 0.05f, 0.95f);
                            // Bound Mouse Cursor Position
                            Vector2 new_mousePosition = Camera.main.WorldToScreenPoint(vertices[0] + (vertices[3] - vertices[0]) * u + (vertices[1] - vertices[0]) * v).XY();
                            Mouse.current.WarpCursorPosition(new_mousePosition);
                        }
                    } 
                }
            }
        }
    }

    public void AnimateBow()
    {
        Draw_Base_Line draw_base_line = base_line.GetComponent<Draw_Base_Line>();
        if (draw_base_line.isShown())
        {
            Vector3 dirVec = draw_base_line.dirVec;
            Vector3 normalVec = draw_base_line.normalVec;
            bow_object.transform.position = draw_base_line.intersect + u * draw_base_line.Length * dirVec + v * (draw_base_line.Depth) * normalVec;
            bow_object.transform.rotation = Quaternion.LookRotation(dirVec);
        } else 
        {
            bow_object.transform.position = new Vector3(1.64f, 0.401f, 2.621f);
            bow_object.transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    public void PlayString()
    {
        Draw_Base_Line draw_base_line = base_line.GetComponent<Draw_Base_Line>();
        if (draw_base_line.isShown())
        {
            GameObject selected = selected_string.GetComponent<Load_Selected_String>().Selected_String;
            Audio_Handler audio_handler_script = selected.GetComponent<Audio_Handler>();
            StringClass string_class = selected.GetComponent<StringClass>();
            audio_handler_script.SetFrequency(Util_Audio.OctabeFreq(string_class.Base_Freq, string_class.Base_idx, pitch_handler.octave, pitch_handler.finger, pitch_handler.offset));
            SimpleSineGenerator audio_script = audio_handler_script.audio_script;
            
            //audio_handler_script.SetFrequency(util_audio.OctabeFreq(selected.GetComponent<StringClass>().Base_Freq, pitch_handler.octave, pitch_handler.finger));
            audio_handler_script.SetAudio(0.1f);   
            audio_script.LoadBowInfo(du, v);
            audio_script.con = 1;
        } else
        {
        }
    }
}