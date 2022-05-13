using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicInput : MonoBehaviour
{
    [SerializeField]
    public string noteNumber = "none";
    private string[] noteNames = { "C", "C ♯", "D", "D ♯", "E", "F", "F ♯", "G", "G ♯", "A", "A ♯", "B" };
    void Start()
    {
        Application.RequestUserAuthorization(UserAuthorization.Microphone);
        AudioSource aud = GetComponent<AudioSource>();   
        aud.clip = Microphone.Start(null, true, 10, 44100);
        aud.Play();
    }

    void Update()
    {
        float[] spectrum = new float[256];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        var maxIndex = 0;
        var maxValue = 0.0f;
        for (int i = 0; i < spectrum.Length; i++)
        {
            var val = spectrum[i];
            if (val > maxValue)
            {
                maxValue = val;
                maxIndex = i;
            }
        }
        var freq = maxIndex * AudioSettings.outputSampleRate / 2 / spectrum.Length;
        noteNumber = GetNoteName(freq);
    }

    public string GetNoteName(float freq)
    {
        //   MIDI ー ー  
        var noteNumber = CalculateNoteNumberFromFrequency(freq);
        // 0:C - 11:B    
        var note = noteNumber % 12;
        // 0:C～11:B       
        return noteNames[note];
    }

    public static int CalculateNoteNumberFromFrequency(float freq)
    {
        return Mathf.FloorToInt(69 + 12 * Mathf.Log(freq / 440, 2));
    }
}
