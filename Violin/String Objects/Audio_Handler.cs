using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class Audio_Handler : MonoBehaviour
{
    public GameObject audio_source_object;
    public AudioSource audio_source;
    public SimpleSineGenerator audio_script;
    public float frequency;

    public float start_time;
    public float time_interval;

    void Awake()
    {
        audio_source = audio_source_object.GetComponent<AudioSource>();
        audio_script = audio_source_object.GetComponent<SimpleSineGenerator>();
        if (audio_source == null)
        {
            audio_source_object.AddComponent<AudioSource>();
        }
        frequency = audio_script.frequency;
    }

    void Update()
    {
        if (Time.time - start_time >= time_interval)
        {
            audio_source.Pause();
        }
    }

    public void SetFrequency(float new_frequency)
    {
        audio_script.frequency = new_frequency;
        frequency = new_frequency;
    }

    public void PlayAudio(float length)
    {
        SetAudio(length);
        audio_source.Play();
    }

    public void SetAudio(float length)
    {
        start_time = Time.time;
        audio_script._start_time = Time.time;
        audio_script._time = Time.time;
        time_interval = length;
    }
}
