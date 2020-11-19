﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

// TODO: consider making areas not inherit from Trigger interface, let them just be AREAS and have a different setup in the Day script
public class AreaTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public event EventHandler OnAreaEnter;
    public event EventHandler OnAreaLeave;

    public bool areaEnabled = true;
    public string areaTag;

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
    }

    void Update()
    {

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
        taskHasBegun = true;
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
            EnableTriggersInside();
            // interactManager.AreaEnter(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            DisableTriggersInside();
            // interactManager.AreaLeave(this);
        }
    }

    void EnableTriggersInside()
    {
        foreach(Trigger trigger in triggersInside)
        {
            trigger.Enable();
        }
    }

    void DisableTriggersInside()
    {
        foreach(Trigger trigger in triggersInside)
        {
            trigger.Disable();
        }
    }
}
