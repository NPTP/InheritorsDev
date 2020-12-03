using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    InteractManager interactManager;

    public event EventHandler OnAreaEnter;
    public event EventHandler OnAreaLeave;

    [Header("Area-specific options")]
    public bool areaEnabled = true;
    public TaskType taskType;
    public string areaTag;
    public bool enableTriggersOnEnter = true;
    [Space]
    [Header("Optional auto-acquire task tool")]
    public ItemType taskTool;

    List<Trigger> triggersInside = new List<Trigger>();
    Collider areaCollider;
    bool removed = false;

    public bool StartedEnabled()
    {
        return areaEnabled;
    }

    void Awake()
    {
        interactManager = FindObjectOfType<InteractManager>();
        areaCollider = GetComponent<Collider>();
    }

    void Start()
    {
        CaptureTriggers();
        TurnOffTriggers();
        if (areaEnabled) Enable();
        else Disable();
    }

    void CaptureTriggers()
    {
        // Find the triggers inside this area.
        IEnumerable<Trigger> allTriggers = FindObjectsOfType<MonoBehaviour>().OfType<Trigger>();
        if (GetComponent<SphereCollider>())
        {
            float areaRadius = areaCollider.bounds.extents.x;
            foreach (Trigger trigger in allTriggers)
            {
                if (trigger != this && (trigger.GetPosition() - transform.position).magnitude <= areaRadius)
                    triggersInside.Add(trigger);
            }
        }
        else if (GetComponent<BoxCollider>())  // NOTE! ***** This only works as an AABB, not on a rotated box!
        {
            foreach (Trigger trigger in allTriggers)
            {
                if (trigger != this && areaCollider.bounds.Contains(trigger.GetPosition()))
                    triggersInside.Add(trigger);
            }
        }
    }

    void TurnOffTriggers()
    {
        foreach (Trigger trigger in triggersInside)
        {
            if (trigger != null) trigger.Disable();
        }
    }

    void TurnOnTriggers()
    {
        foreach (Trigger trigger in triggersInside)
        {
            if (trigger.StartedEnabled())
                trigger.Enable();
        }
    }

    public string GetTag()
    {
        return areaTag;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void ShutDownArea()
    {
        if (removed) return;
        TurnOffTriggers();
        Disable();
    }

    public void StartUpArea()
    {
        if (removed) return;
        TurnOnTriggers();
        Enable();
    }

    public void Enable()
    {
        if (removed) return;

        areaEnabled = true;
        areaCollider.enabled = true;
    }

    public void Disable()
    {
        areaEnabled = false;
        areaCollider.enabled = false;
    }

    // Area removal doesn't truly get rid of it
    public void Remove()
    {
        Disable();
        removed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            interactManager.AreaEnter(this, ref triggersInside, taskTool);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            interactManager.AreaLeave(this, ref triggersInside);
        }
    }
}
