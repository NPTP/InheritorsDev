using System;

// Interface for enabling/disabling different types of triggers in each day script.
public interface Trigger
{
    event EventHandler OnTriggerActivate;
    string GetTag();
    void Enable();
    void Disable();
    void Remove();
}
