using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pizzicato_Handler : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;

    public GameObject selected_string;

    private Pitch_Handler pitch_handler;

    void Awake()
    {
        pitch_handler = GetComponent<Pitch_Handler>();
    }

    public void Run()
    {
        // When player clicks left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            // Circle Range Detection
            GameObject nearest_object = Util_Camera.FindClosestStringInCircleRange(Input.mousePosition, 30f);
            if (nearest_object != null)
            {
                // Add Animation
                Animation_Hanlder Handler_Script = nearest_object.GetComponent<Animation_Hanlder>();
                Animation_Instance Original = Handler_Script.animation_instance.GetComponent<Animation_Instance>();
                Handler_Script.AddAnimation(Original, nearest_object, 0.1f, Util_String.GetKofString(nearest_object, Input.mousePosition), 3f);

                // Add Audio
                Audio_Handler audio_handler_script = nearest_object.GetComponent<Audio_Handler>();
                StringClass string_class = nearest_object.GetComponent<StringClass>();
                audio_handler_script.SetFrequency(Util_Audio.OctabeFreq(string_class.Base_Freq, string_class.Base_idx, pitch_handler.octave, pitch_handler.finger, pitch_handler.offset));
                audio_handler_script.PlayAudio(3.0f);
                audio_handler_script.audio_script.con = 0;

                selected_string.GetComponent<Load_Selected_String>().Selected_String = nearest_object;
            }
            
            // Raycast Detection
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // Play Particle System
                ParticleSystem sparkle = nearest_object.transform.parent.GetComponentInChildren<ParticleSystem>();
                sparkle.transform.position = hit.point;
                sparkle.transform.forward = Vector3.RotateTowards(sparkle.transform.forward, hit.normal, 0f, 0f);
                sparkle.Play();
            }
        }
    }

    

    
}
