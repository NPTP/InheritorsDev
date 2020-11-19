using System;
using UnityEngine;

// Interface for enabling/disabling different types of triggers in each day script.
public interface Trigger
{
    string GetTag();
    Vector3 GetPosition();
    void Enable();
    void Disable();
    void Remove();
}
