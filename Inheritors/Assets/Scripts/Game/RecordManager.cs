/* INHERITORS by Nick Perrin (c) 2020 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The container class for every detail that goes into one frame or "sample"
// of a recording. We can increase sample rate or lower it and interpolate
// between them.
[System.Serializable]
public class Sample
{
    public bool isGrounded;
    public Vector3 direction;
    public Vector3 position;
    public Quaternion rotation;
}

// TODO: use a ring buffer instead of a List
[System.Serializable]
public class SampleBuffer
{
    public List<Sample> buf; // public Sample[] buf;
    bool full = false;
    public int length;
    int start;
    int end;

    public SampleBuffer(int size)
    {
        buf = new List<Sample>(); // buf = new Sample[size];
        length = 0;
        start = 0;
        end = 0;
    }

    public void Add(Sample sample)
    {
        buf.Add(sample);
        length = buf.Count;
    }

    public Sample Get(int index)
    {
        return buf[index];
    }
}

public class RecordManager : MonoBehaviour
{
    StateManager stateManager;
    PlayerMovement playerMovement;
    int maxRecLength = 1024;
    List<SampleBuffer> loadedRecordings = new List<SampleBuffer>();
    List<SampleBuffer> newRecordings = new List<SampleBuffer>();
    SampleBuffer sb;
    bool recording;
    bool hasRecordings = false;

    public GameObject ghostPrefab;
    public bool debugMode = false;

    // For use in SAVING only.
    public List<SampleBuffer> GetCombinedRecordings()
    {
        loadedRecordings.AddRange(newRecordings);
        return loadedRecordings;
    }

    public void LoadRecordings(List<SampleBuffer> loadedRecordings)
    {
        this.loadedRecordings = loadedRecordings;
        hasRecordings = true;
    }

    void Awake()
    {
        stateManager = FindObjectOfType<StateManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    public void Begin()
    {
        if (hasRecordings)
            StartCoroutine(Playback());
    }

    void FixedUpdate()
    {
        if (recording)
        {
            Sample s = playerMovement.GetSample();
            sb.Add(s);
        }
    }

    public void StartNewRecording()
    {
        print("Starting new recording ...");
        sb = new SampleBuffer(maxRecLength);
        recording = true;
    }

    public void StopRecording()
    {
        if (recording)
        {
            print("Stopping recording ...");
            recording = false;
            if (newRecordings == null) { newRecordings = new List<SampleBuffer>(); }
            newRecordings.Add(sb);
            if (debugMode)
            {
                print("We have " + newRecordings.Count + " newRecordings.\nLast recording is " + sb.length + " frames.");
                StartCoroutine(Playback());
            }
        }
        else
        {
            print("Called StopRecording() in RecordManager without starting a recording.");
        }
    }

    IEnumerator Playback()
    {
        yield return null;
        while (hasRecordings && loadedRecordings.Count > 0)
        {
            foreach (SampleBuffer sb in loadedRecordings)
            {
                print("Starting playback with " + loadedRecordings.Count + " loadedRecordings saved");
                GameObject newGhost = Instantiate(ghostPrefab, sb.Get(0).position, sb.Get(0).rotation);
                newGhost.GetComponent<Ghost>().PassBuffer(sb);
                yield return new WaitForSeconds(Random.Range(5, 25));
            }
        }
    }

    // void Update()
    // {
    //     if (debugMode)
    //     {
    //         if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("B"))
    //         {
    //             if (!recording)
    //             {
    //                 StartNewRecording();
    //             }
    //             else
    //             {
    //                 StopRecording();
    //             }
    //         }
    //     }
    // }

}
