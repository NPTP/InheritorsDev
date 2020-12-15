/* INHERITORS by Nick Perrin (c) 2020 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    StateManager stateManager;
    PlayerMovement playerMovement;
    int maxRecLength = 2048;
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

    public void PlayRecordings()
    {
        if (hasRecordings)
        {
            print("PlayRecordings() called");
            StartCoroutine(Playback());
        }
    }

    public void PlayRecordingsSimultaneous()
    {
        if (hasRecordings)
        {
            print("PlayRecordingsSimultaneous() called");
            StartCoroutine(Playback(true));
        }
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
        if (!recording)
        {
            print("Starting new recording ...");
            sb = new SampleBuffer(maxRecLength);
            recording = true;
        }
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
                StartCoroutine(Playback());
            }
        }
        else
        {
            print("Called StopRecording() in RecordManager without starting a recording.");
        }
    }

    IEnumerator Playback(bool simultaneous = false)
    {
        yield return null;

        float minTime = 1.0f;
        float maxTime = 10.0f;

        if (hasRecordings && loadedRecordings.Count > 0)
        {
            foreach (SampleBuffer sb in loadedRecordings)
            {
                GameObject newGhost = Instantiate(ghostPrefab, sb[0].position, sb[0].rotation);
                newGhost.GetComponent<Ghost>().InitializeGhost(sb);
                if (!simultaneous) { yield return new WaitForSeconds(Random.Range(minTime, maxTime)); }
            }
        }
    }

}
