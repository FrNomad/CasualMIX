using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSineGenerator : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float amplitude = 0.5f;
    public float frequency = 440.0f; // A5

    public float _start_time;
    public float _time;
    public float _intensity;

    private double _phase;
    private int _sampleRate;

    public int idx = 0;
    private float[] _spectrum_dist;
    private float[] _spectrum_dist_e = {1.0f,0.1503661529037901f,0.5102174202538381f,0.1226077470133779f,0.2695834724822945f,0.1243274016850048f,0.0863275223251398f,0.1698432554181753f,0.0705788428984518f,0.0576539112722385f,0.1035286498168994f,0.0365359954379929f};
    private float[] _spectrum_dist_a = {1.0f,0.1327018416996154f,0.2177400553105633f,0.0527394202519190f,0.1186762958863448f,0.0719445010888386f,0.1774604083975669f,0.0834453927025880f};
    private float[] _spectrum_dist_d = {1.0f,0.1345207138630396f,0.2013891328537145f,0.0765479874769668f,0.0533446187328696f,0.0189795658718496f,0.1592164224257775f,0.0816068707729278f};
    private float[] _spectrum_dist_g = {0.89582487f, 0.62974112f, 0.29179208f, 0.1320447f, 0.16514187f, 0.13525872f, 0.15204899f, 0.11730179f, 0.07993684f, 0.1080479f, 0.10635081f, 0.01853151f};

    public int con = 0;

    public float velocity, depth, offset_velocity;

    private void Awake()
    {
        _sampleRate = AudioSettings.outputSampleRate;
        Distribute();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {   
            frequency *= Mathf.Pow(2, (float)1/12);
            Debug.Log(frequency);
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            frequency /= Mathf.Pow(2, (float)1/12);
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        double phaseIncrement = frequency / _sampleRate;

        if (con == 1)
        {
             _intensity = 0.5f * (2f - depth) * velocity * offset_velocity;
        }

        for (int sample = 0; sample < data.Length; sample += channels)
        {
            float value = 0f;

            for (int harmonic = 1; harmonic <= _spectrum_dist.Length; harmonic++)
            {
                value += amplitude * _spectrum_dist[harmonic-1] * Mathf.Sin(2 * Mathf.PI * (float) harmonic * (float) _phase);
            }

            _phase = (_phase + phaseIncrement) % 1;

            _time = _time + (float)phaseIncrement / frequency;

            if (con == 0)
            {
                _intensity = Util_Audio.Tanh(_time - _start_time, 1f, 1f);
            }

            for (int channel = 0; channel < channels; channel++)
            {
                data[sample + channel] = _intensity * value;
            }
        }
    }

    private void Distribute()
    {
        if (idx == 0)
        {
            _spectrum_dist = _spectrum_dist_e;
        } else if (idx == 1)
        {
            _spectrum_dist = _spectrum_dist_a;
        } else if (idx == 2)
        {
            _spectrum_dist = _spectrum_dist_d;
        } else if (idx == 3)
        {
            _spectrum_dist = _spectrum_dist_g;
        }
    }

    public void LoadBowInfo(float du, float d)
    {
        velocity = du;
        depth = d;
    }
}
