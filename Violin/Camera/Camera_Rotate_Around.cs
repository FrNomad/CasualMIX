using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Rotate_Around : MonoBehaviour
{
    public float sensitivity = 1.0f;
    public float radius = 10f;
    public float pitch = 0f;
    public float law = 0f;

    void Start()
    {
        law = transform.localRotation.eulerAngles.y;
        pitch = transform.localRotation.eulerAngles.x;
    }

    public void Update()
    {
        if (Input.GetMouseButton(1))
        {
            float dx = Input.GetAxis("Mouse X") * sensitivity;
            float dy = Input.GetAxis("Mouse Y") * sensitivity;
            pitch -= sensitivity * dy;
            law += sensitivity * dx;

            if (Input.mousePosition.x > Screen.width)
            {
                law += sensitivity * 1;
            } else if (Input.mousePosition.x < 0)
            {
                law -= sensitivity * 1;
            }

            if (Input.mousePosition.y > Screen.height)
            {
                pitch -= sensitivity * 1;
            } else if (Input.mousePosition.y < 0)
            {
                pitch += sensitivity * 1;
            }
        }
        Quaternion rot = Quaternion.Euler(pitch, law, 0);
        transform.rotation = rot;

        radius -= Input.mouseScrollDelta.y;
    }

    public void LateUpdate()
    {
        transform.position = new Vector3(0, 0, 0);
        transform.position -= radius * Camera.main.transform.forward;
    }
}
