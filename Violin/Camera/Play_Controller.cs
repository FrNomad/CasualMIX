using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play_Controller : MonoBehaviour
{
    public string play_mode;

    public Pizzicato_Handler Pizzicato_Handler_Script;
    public Detache_Handler Detache_Handler_Script;

    void Start()
    {
        play_mode = "Pizzicato";
        Pizzicato_Handler_Script = GetComponent<Pizzicato_Handler>();
        Detache_Handler_Script = GetComponent<Detache_Handler>();
    }

    void Update()
    {
        ChangePlayMode();

        switch (play_mode)
        {
            case "Pizzicato":
                Pizzicato_Handler_Script.Run();
                break;
            case "Detache":
                Detache_Handler_Script.Run();
                Detache_Handler_Script.Interact();
                Detache_Handler_Script.AnimateBow();
                Detache_Handler_Script.PlayString();
                break;
            default: 
                Debug.Log("Error : No matched play method exists");
                break;
        }
    }

    public void ChangePlayMode()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            play_mode = "Pizzicato";
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            play_mode = "Detache";
        }
    }

    public void ChangeToPizz()
    {
        play_mode = "Pizzicato";
    }

    public void ChangeToDetache()
    {
        play_mode = "Detache";
    }
}
