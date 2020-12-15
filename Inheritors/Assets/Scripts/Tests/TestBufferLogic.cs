using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestBufferLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Test wrap-around
        TestBuffer testBuffer1 = new TestBuffer(5);
        testBuffer1.Add(1);
        testBuffer1.Add(2);
        testBuffer1.Add(3);
        testBuffer1.Add(4);
        testBuffer1.Add(5);
        testBuffer1.Add(6);

        print("====================================");
        print(testBuffer1.ReturnContentsString());
        print("Full: " + testBuffer1.full);
        print("Start index at: " + testBuffer1.start);
        print("Length is: " + testBuffer1.length);
        print("Size is: " + testBuffer1.size);
        testBuffer1.Add(7);
        print(testBuffer1.ReturnContentsString());
        print("Full: " + testBuffer1.full);
        print("Start index at: " + testBuffer1.start);
        print("Length is: " + testBuffer1.length);
        print("Size is: " + testBuffer1.size);
        testBuffer1.Add(8);
        testBuffer1.Add(9);
        testBuffer1.Add(10);
        print(testBuffer1.ReturnContentsString());
        print("Full: " + testBuffer1.full);
        print("Start index at: " + testBuffer1.start);
        print("Length is: " + testBuffer1.length);
        print("Size is: " + testBuffer1.size);
        testBuffer1.Add(11);
        print(testBuffer1.ReturnContentsString());
        print("Full: " + testBuffer1.full);
        print("Start index at: " + testBuffer1.start);
        print("Length is: " + testBuffer1.length);
        print("Size is: " + testBuffer1.size);
        print("====================================");

        // Test length < full
        TestBuffer testBuffer2 = new TestBuffer(10);
        testBuffer2.Add(1);
        testBuffer2.Add(2);
        testBuffer2.Add(3);
        testBuffer2.Add(4);
        testBuffer2.Add(5);
        testBuffer2.Add(6);
        testBuffer2.Add(7);
        testBuffer2.Add(8);

        print("====================================");
        print(testBuffer2.ReturnContentsString());
        print("Full: " + testBuffer2.full);
        print("Start index at: " + testBuffer2.start);
        print("Length is: " + testBuffer2.length);
        print("Size is: " + testBuffer2.size);
        print("====================================");

        testBuffer2.Add(9);
        testBuffer2.Add(10);

        print("====================================");
        print(testBuffer2.ReturnContentsString());
        print("Full: " + testBuffer2.full);
        print("Start index at: " + testBuffer2.start);
        print("Length is: " + testBuffer2.length);
        print("Size is: " + testBuffer2.size);
        print("====================================");

        testBuffer2.Add(11);

        print("====================================");
        print(testBuffer2.ReturnContentsString());
        print("Full: " + testBuffer2.full);
        print("Start index at: " + testBuffer2.start);
        print("Length is: " + testBuffer2.length);
        print("Size is: " + testBuffer2.size);
        print("====================================");

        print("[3] == " + testBuffer2[3]);
        print("[9] == " + testBuffer2[9]);
        print("[10]: ");
        print(testBuffer2[10]);
    }



    public class TestBuffer
    {
        public int[] buf;
        public bool full = false;

        public int size;
        public int length;
        public int start;
        public int insert;

        public string ReturnContentsString()
        {
            string s = "";
            for (int i = 0; i < length; i++)
            {
                s += buf[i];
                if (i < length - 1)
                    s += ", ";
            }
            return s;
        }

        public TestBuffer(int size)
        {
            buf = new int[size];
            this.size = size;
            length = 0;
            start = 0;
            insert = 0;
        }

        public void Add(int num)
        {
            if (!full)
            {
                buf[insert] = num;
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
                buf[insert] = num;
                start = insert + 1;
                insert++;

                if (start == size)
                {
                    start = 0;
                    insert = 0;
                }
            }
        }

        // Behaves like indexing into a normal array. GETTING ONLY!
        public int this[int index]
        {
            get
            {
                if (index >= length)
                {
                    throw new ArgumentOutOfRangeException("index", "index >= size for TestBuffer");
                }
                else
                {
                    return buf[(index + start) % size];
                }
            }
        }

        public int Head
        {
            get => buf[start];
        }

        public int Length
        {
            get => this.length;
            // set
            // { }
        }

    }
}
