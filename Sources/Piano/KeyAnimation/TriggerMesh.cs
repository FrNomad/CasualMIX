using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMesh : MonoBehaviour
{
    public int id;

    private Ray ray;
    private RaycastHit hit;

    private Camera camera;
    private Piano_Mouse_Controller mouse_controller;


    public bool isEnter, isExit;
    private GameObject prev_selected_key;

    void Awake()
    {
        camera = Camera.main;
        mouse_controller = camera.GetComponent<Piano_Mouse_Controller>();

        isEnter = false;
        isExit = false;
    }

    void Update()
    {
        UpdateEventStates();
        if (isEnter)
        {
            GameEvent.current.MouseEnter(id);
        }

        if (isExit)
        {
            GameEvent.current.MouseExit(id);
        }
    }

    void UpdateEventStates()
    {
        if (prev_selected_key != gameObject && mouse_controller.HoveredKey == gameObject)
        {
            isEnter = true;
        } else
        {
            isEnter = false;
        }

        if (prev_selected_key == gameObject && mouse_controller.HoveredKey != gameObject)
        {
            isExit = true;
        } else
        {
            isExit = false;
        }

        prev_selected_key = mouse_controller.HoveredKey;
    }
}
