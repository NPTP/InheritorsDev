using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

// TODO: consider making areas not inherit from Trigger interface, let them just be AREAS and have a different setup in the Day script
public class AreaTrigger : MonoBehaviour, Trigger
{
    public void FlagInArea(AreaTrigger area) { }

    InteractManager interactManager;

    public event EventHandler OnAreaEnter;
    public event EventHandler OnAreaLeave;

    [Header("Area-specific options")]
    public bool areaEnabled = true;
    public string areaTag;
    [Space]
    [Header("Optional task tool appearing in area")]
    public GameObject taskTool;

    [HideInInspector]
    public bool taskHasBegun = false;
    List<Trigger> triggersInside = new List<Trigger>();

    Collider areaCollider;

    void Awake()
    {
        interactManager = FindObjectOfType<InteractManager>();
    }

    void Start()
    {
        areaCollider = GetComponent<Collider>();
        CaptureTriggers();

        if (areaEnabled) Enable();
        else Disable();
    }

    void CaptureTriggers()
    {
        // Find the triggers inside this area.
        IEnumerable<Trigger> allTriggers = allTriggers = FindObjectsOfType<MonoBehaviour>().OfType<Trigger>();
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

        // Now tell those triggers they are inside this area.
        foreach (Trigger triggerInside in triggersInside)
        {
            triggerInside.FlagInArea(this);
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

    public void BeginTaskInArea()
    {
        if (!taskHasBegun)
        {
            Disable();
            taskHasBegun = true;
        }
    }

    public bool HasTaskBegun()
    {
        return taskHasBegun;
    }

    public void Enable()
    {
        areaEnabled = true;
        areaCollider.enabled = true;
    }

    public void Disable()
    {
        areaEnabled = false;
        areaCollider.enabled = false;
    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // EnableTriggersInside();
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
