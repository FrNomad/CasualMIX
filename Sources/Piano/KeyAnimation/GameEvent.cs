using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour
{
    public static GameEvent current;

    private void Awake()
    {
        current = this;
    }

    public event Action<int> onMouseEnter;
    public void MouseEnter(int id)
    {
        if (onMouseEnter != null)
        {
            onMouseEnter(id);
        }
    }

    public event Action<int> onMouseExit;
    public void MouseExit(int id)
    {
        if (onMouseExit != null)
        {
            onMouseExit(id);
        }
    }
}
