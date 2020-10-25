using System;
using UnityEngine;
using UnityEngine.Events;

public class TestingEvents : MonoBehaviour
{
    public event EventHandler<OnSpacePressedEventArgs> OnSpacePressed;
    public class OnSpacePressedEventArgs : EventArgs
    {
        public int spaceCount;
    }

    public UnityEvent OnUnityEvent;

    private int spaceCount;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Space pressed!
            spaceCount++;
            OnSpacePressed?.Invoke(this, new OnSpacePressedEventArgs { spaceCount = spaceCount });

            OnUnityEvent?.Invoke();
        }
    }

}
