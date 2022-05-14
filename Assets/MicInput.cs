using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class MicInput : MonoBehaviour
{
    public int noOfSegments;
    float threshold = -20.00f;
    int qSamples = 1024;
    [SerializeField]
    public AudioMixer masterMixer;
    public Text counter;
    public Text noteName;
    public Slider progressBar;
    private string[] noteNames = { "C", "C ♯", "D", "D ♯", "E", "F", "F ♯", "G", "G ♯", "A", "A ♯", "B" };
    int timeout = 3;
    float nextChange;
    int segmentsLeft;
    private string state;
    void Start()
    {
        Application.RequestUserAuthorization(UserAuthorization.Microphone);
        AudioSource aud = GetComponent<AudioSource>();   
        aud.clip = Microphone.Start(null, true, 100, 44100);
        aud.Play();
        // nextChange = Time.realtimeSinceStartup + timeout;
        // state = "timeout";

    }

    void Update()
    {
        Analyze();
        // if (nextChange < Time.realtimeSinceStartup ) {
        //     if (state == "timeout") {
        //         segmentsLeft = noOfSegments;
        //         state = "registeringNote";
        //         counter.enabled = false;
        //         RegisterNote();
        //         return;
        //     }
        //     if (state == "registeringNote") {
        //         Analyze();
        //         if (segmentsLeft > 0) {
        //             RegisterNote();
        //         }
        //         else {
        //             state = "timeout";
        //             counter.enabled = true;
        //             nextChange = Time.realtimeSinceStartup + timeout;
        //             noteName.text = "";
        //             progressBar.value = 0;
        //         }
        //     }
        // }
        // if (state == "registeringNote") {
        //     progressBar.value = (Time.realtimeSinceStartup - nextChange + 1);
        // }
        // if (state == "timeout") {
        //     counter.text = Mathf.CeilToInt(nextChange - Time.realtimeSinceStartup).ToString();
        // }
        
    }

    void RegisterNote() {
        // AudioSource aud = GetComponent<AudioSource>();   
        // aud.clip = Microphone.Start(null, true, 1, 44100);
        // aud.Play();
        // nextChange = Time.realtimeSinceStartup + 1;
        // segmentsLeft--;
    }

    void Analyze()
    {
        float[] spectrum = new float[qSamples];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float maxV = 0;
        int maxN = 0;
        for (int i = 0; i < qSamples; i++)
        { // find max 
            if (spectrum[i] > maxV && spectrum[i] > threshold)
            {
                maxV = spectrum[i];
                maxN = i; // maxN is the index of max
            }
        }
        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < qSamples - 1)
        { // interpolate index using neighbours
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += (float)0.5 * (dR * dR - dL * dL);
        }
        var pitchValue = freqN * (AudioSettings.outputSampleRate / 2) / qSamples; // convert index to frequency
        var name = GetNoteName(pitchValue);
        if (name != "none") {
            noteName.text = GetNoteName(pitchValue);
        }
    }

    public string GetNoteName(float freq)
    {
        //   MIDI ー ー  
        var noteNumber = CalculateNoteNumberFromFrequency(freq);
        if (noteNumber < 0)
        {
            return "none";
        }
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
