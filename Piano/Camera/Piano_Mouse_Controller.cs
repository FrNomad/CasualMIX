using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piano_Mouse_Controller : MonoBehaviour
{
    public GameObject HoveredKey;
    private Ray ray;
    private RaycastHit hit;

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "WhiteKey" || hit.collider.gameObject.tag == "BlackKey")
                {
                    HoveredKey = hit.collider.gameObject;
                } else
                {
                    HoveredKey = null;
                }
            } else 
            {
                HoveredKey = null;
            }
        } else
        {
            HoveredKey = null;
        }
    }
}
