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

// We expect no SampleBuffer of size < 3
// (and realistically none less than 1024)
[System.Serializable]
public class SampleBuffer
{
    public Sample[] buf;
    bool full = false;

    int size;
    int length;
    public int start;
    int insert;

    public SampleBuffer(int size)
    {
        buf = new Sample[size];
        this.size = size;
        length = 0;
        start = 0;
        insert = 0;
    }

    public void Add(Sample sample)
    {
        if (!full)
        {
            buf[insert] = sample;
            insert++;
            length++;

            if (length == size)
            {
                full = true;
                insert = 0;
            }
        }
        else
        {
            buf[insert] = sample;
            start = insert + 1;
            insert++;

            if (start == size)
            {
                start = 0;
                insert = 0;
            }
        }
    }

    // Behaves like indexing into a normal array, but GET only!
    public Sample this[int index]
    {
        get
        {
            if (index >= size)
            {
                throw new ArgumentOutOfRangeException("index", "index >= size for SampleBuffer");
            }
            else
            {
                return buf[(index + start) % size];
            }
        }
    }

    public Sample Head
    {
        get => buf[start];
    }

    public int Length
    {
        get => this.length;
    }
}




// LIST IMPLEMENTATION
//
// [System.Serializable]
// public class SampleBuffer
// {
//     public List<Sample> buf; // public Sample[] buf;
//     bool full = false;

//     int length;
//     int start;
//     int end;

//     public SampleBuffer(int size)
//     {
//         buf = new List<Sample>(); // buf = new Sample[size];
//         length = 0;
//         start = 0;
//         end = 0;
//     }

//     public void Add(Sample sample)
//     {
//         buf.Add(sample);
//         length = buf.Count;
//     }

//     public Sample Get(int index)
//     {
//         return buf[index];
//     }

//     public int Length
//     {
//         get
//         {
//             return this.length;
//         }
//         set
//         { }
//     }
// }
