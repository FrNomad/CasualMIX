using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piano_Keyboard_Controller : MonoBehaviour
{
    public GameObject Piano;
    private GameObject WhiteKeys, BlackKeys;
    [SerializeField] private int octave;

    [SerializeField] private List<int> press_down_keys, press_up_keys;

    void Awake()
    {
        octave = 1;

        press_down_keys = new List<int>();
        press_up_keys = new List<int>();

        WhiteKeys = Piano.transform.Find("White_keys").gameObject;
        BlackKeys = Piano.transform.Find("Black_keys").gameObject;
    }

    void FixedUpdate()
    {
        // Press Down & Up Keys
        press_down_keys = new List<int>();
        press_up_keys = new List<int>();

        if (Input.GetKeyDown(KeyCode.A))
        {
            press_down_keys.Add(0);
        } else if (Input.GetKeyUp(KeyCode.A))
        {
            press_up_keys.Add(0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            press_down_keys.Add(1);
        } else if (Input.GetKeyUp(KeyCode.S))
        {
            press_up_keys.Add(1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            press_down_keys.Add(2);
        } else if (Input.GetKeyUp(KeyCode.D))
        {
            press_up_keys.Add(2);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            press_down_keys.Add(3);
        } else if (Input.GetKeyUp(KeyCode.F))
        {
            press_up_keys.Add(3);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            press_down_keys.Add(4);
        } else if (Input.GetKeyUp(KeyCode.G))
        {
            press_up_keys.Add(4);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            press_down_keys.Add(5);
        } else if (Input.GetKeyUp(KeyCode.H))
        {
            press_up_keys.Add(5);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            press_down_keys.Add(6);
        } else if (Input.GetKeyUp(KeyCode.J))
        {
            press_up_keys.Add(6);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            press_down_keys.Add(7);
        } else if (Input.GetKeyUp(KeyCode.W))
        {
            press_up_keys.Add(7);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            press_down_keys.Add(8);
        } else if (Input.GetKeyUp(KeyCode.E))
        {
            press_up_keys.Add(8);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            press_down_keys.Add(9);
        } else if (Input.GetKeyUp(KeyCode.T))
        {
            press_up_keys.Add(9);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            press_down_keys.Add(10);
        } else if (Input.GetKeyUp(KeyCode.Y))
        {
            press_up_keys.Add(10);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            press_down_keys.Add(11);
        } else if (Input.GetKeyUp(KeyCode.U))
        {
            press_up_keys.Add(11);
        }

        // Octaves
        if (Input.GetKeyDown("1"))
        {
            octave = 1;
        } else if (Input.GetKeyDown("2"))
        {
            octave = 2;
        } else if (Input.GetKeyDown("3"))
        {
            octave = 3;
        } else if (Input.GetKeyDown("4"))
        {
            octave = 4;
        } else if (Input.GetKeyDown("5"))
        {
            octave = 5;
        } else if (Input.GetKeyDown("6"))
        {
            octave = 6;
        } else if (Input.GetKeyDown("7"))
        {
            octave = 7;
        } else if (Input.GetKeyDown("8"))
        {
            octave = 8;
        } else if (Input.GetKeyDown("9"))
        {
            octave = 9;
        } else if (Input.GetKeyDown("0"))
        {
            octave = 0;
        }
    }

    void Update()
    {
        for (int i = 0; i < press_down_keys.Count; i++)
        {
            int idx = KeyIndex(octave, press_down_keys[i]);
            GameObject key;
            if (press_down_keys[i] <= 6)
            {
                key = WhiteKeys.transform.GetChild(idx).gameObject;
            } else
            {
                key = BlackKeys.transform.GetChild(idx).gameObject;
            }
            
            TriggerMesh trigger = key.GetComponent<TriggerMesh>();
            PianoSoundGenerator audio_script = key.AddComponent<PianoSoundGenerator>();
            if (audio_script != null)
            {
                if (press_down_keys[i] <= 6)
                {
                    audio_script.SetFrequency(Util_Audio.WhiteKey_Freq(octave, press_down_keys[i]));
                } else
                {
                    audio_script.SetFrequency(Util_Audio.BlackKey_Freq(octave, press_down_keys[i] - 7));
                }
                audio_script.SetAmplitude(audio_script.amplitude / octave);
                if (Input.GetKey(KeyCode.Space))
                {
                    audio_script.PlayAudio(3f);
                } else 
                {
                    audio_script.PlayAudio(1f);
                }
                
            }
            GameEvent.current.MouseEnter(trigger.id);
            
        }

        for (int i = 0; i < press_up_keys.Count; i++)
        {
            int idx = KeyIndex(octave, press_up_keys[i]);
            GameObject key;
            if (press_up_keys[i] <= 6)
            {
                key = WhiteKeys.transform.GetChild(idx).gameObject;
            } else
            {
                key = BlackKeys.transform.GetChild(idx).gameObject;
            }

            TriggerMesh trigger = key.GetComponent<TriggerMesh>();
            GameEvent.current.MouseExit(trigger.id);
        
        }
    }

    private int KeyIndex(int octave, int key)
    {
        int idx;

        if (key <= 6)
        {
            idx = octave * 7 + key - 5;
            idx = 51 - idx;
            idx = (int)Mathf.Clamp(idx, 0, 51);
        } else
        {
            idx = octave * 5 + (key - 7) - 4;
            idx = 35 - idx;
            idx = (int)Mathf.Clamp(idx, 0, 35);
        }

        return idx;
    }
}
