using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitch_Handler : MonoBehaviour
{
    public float octave, finger, offset;

    void Awake()
    {
        octave = 1f;
    }

    void Update()
    {
        // Finger number & Sharp and Flat
        finger = 0f; offset = 0f;
        if (Input.GetKey(KeyCode.Q))
        {
            finger = 0f; offset = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            finger = 1f; offset = 0f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            finger = 1f; offset = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            finger = 2f; offset = 0f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            finger = 2f; offset = 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            finger = 3f; offset = 0f;
        }
        if (Input.GetKey(KeyCode.R))
        {
            finger = 3f; offset = 1f;
        }
        if (Input.GetKey(KeyCode.F))
        {
            finger = 4f; offset = 0f;
        }
        if (Input.GetKey(KeyCode.T))
        {
            finger = 4f; offset = 1f;
        } 

        // Octaves
        if (Input.GetKeyDown("1"))
        {
            octave = 1f;
        } else if (Input.GetKeyDown("2"))
        {
            octave = 2f;
        } else if (Input.GetKeyDown("3"))
        {
            octave = 3f;
        } else if (Input.GetKeyDown("4"))
        {
            octave = 4f;
        } else if (Input.GetKeyDown("5"))
        {
            octave = 5f;
        } else if (Input.GetKeyDown("6"))
        {
            octave = 6f;
        } else if (Input.GetKeyDown("7"))
        {
            octave = 7f;
        } else if (Input.GetKeyDown("8"))
        {
            octave = 8f;
        } else if (Input.GetKeyDown("9"))
        {
            octave = 9f;
        }
    }
}
