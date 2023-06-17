using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    private float _start_time;
    public int id;

    private void Start()
    {
        GameEvent.current.onMouseEnter += onMouseHoverEnter;
        GameEvent.current.onMouseExit += onMouseHoverExit;
        _start_time = Time.time;
    }

    private void onMouseHoverEnter(int id)
    {
        if (id == this.id)
        {
            _start_time = Time.time;
            if (gameObject.tag == "WhiteKey")
            {
                LeanTween.moveLocalY(gameObject, 0.2196756f-0.03f, 0.2f).setEaseOutQuad();
            } else if (gameObject.tag == "BlackKey")
            {
                LeanTween.moveLocalY(gameObject, 0.2696756f-0.03f, 0.2f).setEaseOutQuad();
            }
        }   
    }

    private void onMouseHoverExit(int id)
    {
        if (id == this.id)
        {
            _start_time = Time.time;
            if (gameObject.tag == "WhiteKey")
            {
                LeanTween.moveLocalY(gameObject, 0.2196756f, 0.2f).setEaseOutQuad();
            } else if (gameObject.tag == "BlackKey")
            {
                LeanTween.moveLocalY(gameObject, 0.2696756f, 0.2f).setEaseOutQuad();
            }
        }
    }

    private void onDestroy()
    {
        GameEvent.current.onMouseEnter -= onMouseHoverEnter;
        GameEvent.current.onMouseExit -= onMouseHoverExit;
    }

    
}
