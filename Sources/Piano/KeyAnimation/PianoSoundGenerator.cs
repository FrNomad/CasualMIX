using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PianoSoundGenerator : MonoBehaviour
{
    // Attributes for audio handling
    public float _start_time;
    public float _time_interval;
    public AudioSource audio_source;

    // Attributes for sound generating
    [Range(0, 1)] public float amplitude = 0.5f;
    public float frequency = 440.0f; // A4

    public float _time;
    public float _intensity;

    private double _phase;
    private int _sampleRate;

    private float[] _spectrum_dist = {0.664021900f, 0.321806783f, 0.375097142f, 0.291634931f, 0.128740885f, 0.0699137425f, 0.0541433345f, 0.00658230653f, 0.00866443871f, 0.00116770055f, 0.00875128001f, 0.00404929822f, 0.00586870070f, 0.00421253330f, 0.00668366305f, 0.00364474782f};

    private void Awake()
    {
        audio_source = GetComponent<AudioSource>();

        _sampleRate = AudioSettings.outputSampleRate;
    }

    private void Update()
    {
        if (Time.time - _start_time >= _time_interval)
        {
            audio_source.Pause();
            Destroy(this);
        }
    }

    public void PlayAudio(float length)
    {
        SetAudio(length);
        audio_source.Play();
    }

    public void SetAudio(float length)
    {
        _start_time = Time.time;
        _time_interval = length;
    }

    public void SetFrequency(float new_frequency)
    {
        frequency = new_frequency;
    }

    public void SetAmplitude(float new_amplitude)
    {
        amplitude = new_amplitude;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        double phaseIncrement = frequency / _sampleRate;

        for (int sample = 0; sample < data.Length; sample += channels)
        {
            float value = 0f;

            for (int harmonic = 1; harmonic <= _spectrum_dist.Length; harmonic++)
            {
                value += amplitude * _spectrum_dist[harmonic-1] * Mathf.Sin(2 * Mathf.PI * (float) harmonic * (float) _phase);
            }

            _phase = (_phase + phaseIncrement) % 1;

            _time = _time + (float)phaseIncrement / frequency;

            _intensity = Util_Audio.Tanh(_time, 1f, _time_interval);

            for (int channel = 0; channel < channels; channel++)
            {
                data[sample + channel] = _intensity * value;
            }
        }
    }
}
