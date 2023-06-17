using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drum_Mouse_Controller : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.tag == "DrumTrigger")
                    {
                        AudioSource audio = hit.collider.gameObject.GetComponent<AudioSource>();
                        audio.Play();
                    }
                }
            }
        }
    }
}
