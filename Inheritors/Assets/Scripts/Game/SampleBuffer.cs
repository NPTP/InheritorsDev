/* INHERITORS by Nick Perrin (c) 2020 */
using System;
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

// TODO: Make this a Ring Buffer WITH A MAX SIZE OF 2000 SAMPLES
[System.Serializable]
public class SampleBuffer
{
    public List<Sample> buf; // public Sample[] buf;
    bool full = false;

    int length;
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

    public int Length
    {
        get
        {
            return this.length;
        }
        set
        { }
    }
}