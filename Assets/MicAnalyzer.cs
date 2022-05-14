using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class MicAnalyzer : MonoBehaviour
{
    public delegate void Signal();
    [SerializeField]
    public Text noteName;
    public Slider progressBar;
    int registrationDuration = 1;
    float voiceRegistrationStartTime;
    public Signal segmentEnded;
    public bool stopped  = true;
    public GameObject singleSegmentAnalyzerPrefab;
    GameObject singleSegmentAnalyzer;
    void Start() {
    }
    public void Reset()
    {
        Debug.Log("Reset");
        singleSegmentAnalyzer = Instantiate(singleSegmentAnalyzerPrefab);
        voiceRegistrationStartTime = Time.realtimeSinceStartup;
        noteName.text = "";
        progressBar.value = 0;
        stopped = false;
        singleSegmentAnalyzer.GetComponent<SingleSegmentAnalyzer>().recordingEnded = Stop;
    }

    void Stop() {
        progressBar.value = 1;
        DestroyImmediate(singleSegmentAnalyzer);
        singleSegmentAnalyzer = null;
        stopped = true;
        if (segmentEnded != null) {
            segmentEnded.Invoke();
        }
    }

    void Update()
    {
        if (stopped) {
            return;
        }
        noteName.text = singleSegmentAnalyzer.GetComponent<SingleSegmentAnalyzer>().mostFrequentNoteName;
        progressBar.value = (Time.realtimeSinceStartup - voiceRegistrationStartTime)/registrationDuration;
    }
}
