using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SimpleStopwatch : MonoBehaviour
{
    [SerializeField]
    private TextMesh timeTextOutput;
    private float time; 
    private bool start = false;

    private Stopwatch stopwatch;
    // Start is called before the first frame update
    void Start()
    {
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            time += Time.deltaTime * 1;
        }
        if (timeTextOutput)
        {
            timeTextOutput.text = time.ToString();
        }
    }

    public void StartTime()
    {
        start = true;
    }

    public void StopTime()
    {
        start = false;
    }

    public void ResetTime()
    {
        start = false;
        time = 0f;
    }
}
