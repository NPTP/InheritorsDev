using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The container class for every detail that goes into one frame or "sample"
// of a recording. We can increase sample rate or lower it and interpolate
// between them.
public class Sample
{
    public Vector3 direction;
    public Vector3 position;
    public Quaternion rotation;
}

public class SampleBuffer
{
    public Sample[] buf;
    bool full = false;
    public int length;
    int start;
    int end;

    public SampleBuffer(int len)
    {
        buf = new Sample[len];
        length = len;
        start = 0;
        end = 0;
    }

    public void Add(Sample sample)
    {
        if (!full && end < length)
        {
            buf[end] = sample;
            end++;
            length = end;
        }

        if (end >= length)
        {
            full = true;
        }
    }
}

public class RecordManager : MonoBehaviour
{
    StateManager stateManager;
    PlayerMovement pm;
    Animator pa;
    int maxRecLength = 1024;
    List<SampleBuffer> recordings = new List<SampleBuffer>();
    SampleBuffer sb;
    bool recording;

    public bool debugMode = false;

    void Awake()
    {
        stateManager = FindObjectOfType<StateManager>();
        pm = FindObjectOfType<PlayerMovement>();
        pa = GameObject.FindWithTag("Player").GetComponent<Animator>();
    }

    void Update()
    {
        if (debugMode)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!recording)
                {
                    StartNewRecording();
                }
                else
                {
                    StopRecording();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (recording)
        {
            Sample s = pm.GetSample();
            sb.Add(s);
        }
    }

    void StartNewRecording()
    {
        sb = new SampleBuffer(maxRecLength);
        recording = true;
    }

    void StopRecording()
    {
        recordings.Add(sb);
        // sb = null;
        recording = false;
        if (debugMode) { StartCoroutine(Playback()); }
    }

    IEnumerator Playback()
    {
        stateManager.SetState(State.Debug);

        foreach (SampleBuffer sb in recordings)
        {
            for (int i = 0; i < sb.length; i++)
            {
                if (sb.buf[i] == null) { break; }
                pm.direction = sb.buf[i].direction;
                pm.transform.position = sb.buf[i].position;
                pm.transform.rotation = sb.buf[i].rotation;
                pa.SetFloat("MoveSpeed", sb.buf[i].direction.magnitude);
                yield return new WaitForFixedUpdate();
            }
        }

        stateManager.SetState(State.Normal);
    }
}
