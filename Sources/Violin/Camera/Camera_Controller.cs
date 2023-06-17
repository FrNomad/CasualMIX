using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public float Speed = 5f;
    public float sensitivity = 2f;
    public float pitch = 0f;
    public float law = 0f;

    void Start()
    {
        law = transform.rotation.eulerAngles.y;
        pitch = transform.rotation.eulerAngles.x;
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += Speed * Time.deltaTime * Camera.main.transform.forward;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= Speed * Time.deltaTime * Camera.main.transform.forward;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 right = Vector3.Cross(Vector3.up, Camera.main.transform.forward).normalized;
            transform.position += Speed * Time.deltaTime * right;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 right = Vector3.Cross(Vector3.up, Camera.main.transform.forward).normalized;
            transform.position -= Speed * Time.deltaTime * right;
        }

        if (Input.GetMouseButton(1))
        {
            float dx = Input.GetAxis("Mouse X");
            float dy = Input.GetAxis("Mouse Y");
            pitch -= sensitivity * dy;
            law += sensitivity * dx;
            transform.eulerAngles = new Vector3(pitch, law, 0);
        }
        
        //Camera.main.fieldOfView -= Input.mouseScrollDelta.y;
    }
}
