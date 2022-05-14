using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MicInput : MonoBehaviour
{

    public int noOfSegments;
    [SerializeField]
    public Text counter;
    int timeout = 3;
    float nextChange;
    int segmentsLeft = -1;
    private string state;
    private float timeoutStartTime;
    private float nextRoundStart;

    
    void Start()
    {
        GetComponent<MicAnalyzer>().segmentEnded += OnSegmentEnded;
        timeoutStartTime = Time.realtimeSinceStartup;
        nextRoundStart = Time.realtimeSinceStartup + timeout;
        Application.RequestUserAuthorization(UserAuthorization.Microphone);
    }

    void Update()
    {
        if (Time.realtimeSinceStartup >= timeoutStartTime && Time.realtimeSinceStartup <= nextRoundStart) {
            if (!counter.enabled) {
                counter.enabled = true;
            }
            counter.text = Mathf.CeilToInt(nextRoundStart - Time.realtimeSinceStartup).ToString();
        }
        else {
            if (counter.enabled) {
                counter.enabled = false;
            }
            if (segmentsLeft == -1) {
                segmentsLeft = 3;
                GetComponent<MicAnalyzer>().Reset();
            }
        }
    }

    void OnSegmentEnded() {
        segmentsLeft--;
        if (segmentsLeft <= 0) {
            segmentsLeft = -1;
            timeoutStartTime = Time.realtimeSinceStartup;
            nextRoundStart = timeoutStartTime + timeout;
            GetComponent<MicAnalyzer>().stopped = true;
        }
        else {
            GetComponent<MicAnalyzer>().Reset();
        }
    }
}
