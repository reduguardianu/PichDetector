using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
public class SingleSegmentAnalyzer:MonoBehaviour {
    Dictionary<string, int> noOfNotes = new Dictionary<string, int>();
    public int registrationDuration = 1;
    float threshold = -20.00f;
    int qSamples = 1024;
    public string mostFrequentNoteName;
    public AudioMixerGroup output;
    private string[] noteNames = { "C", "C ♯", "D", "D ♯", "E", "F", "F ♯", "G", "G ♯", "A", "A ♯", "B" };

    public AudioMixer masterMixer;
    float startTime;
    public delegate void Signal();
    public Signal recordingEnded; 
    void Start() {
        Application.RequestUserAuthorization(UserAuthorization.Microphone);
        AudioSource aud = gameObject.AddComponent<AudioSource>();   
        aud.outputAudioMixerGroup = output;
        aud.clip = Microphone.Start(null, true, 100, 44100);
        while (!(Microphone.GetPosition(null) > 0)) {}
        startTime = Time.realtimeSinceStartup;
        aud.Play();
    }

    void Update() {
        Analyze();
        int highestNoteFrequency = 0;
        mostFrequentNoteName = "none";
        foreach (var item in noOfNotes) {
            if (item.Value > highestNoteFrequency) {
                highestNoteFrequency = item.Value;
                mostFrequentNoteName = item.Key;
            }
        }
        if (startTime + registrationDuration < Time.realtimeSinceStartup) {
            Microphone.GetPosition(null);
            Microphone.End(null);
            recordingEnded.Invoke();
        }
    }

    void Analyze()
    {
        float[] spectrum = new float[qSamples];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float maxV = 0;
        int maxN = 0;
        for (int i = 0; i < qSamples; i++)
        { // find max
            if (spectrum[i] > maxV)
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
            if (noOfNotes.ContainsKey(name)) {
                noOfNotes[name] = noOfNotes[name] + 1;
            }
            else {
                noOfNotes.Add(name, 1);
            }
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