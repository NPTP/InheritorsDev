using System;
using UnityEngine;

public class DialogTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public event EventHandler OnDialogEnterRange;
    public event EventHandler OnDialogLeaveRange;
    public event EventHandler OnTriggerActivate;

    public bool triggerEnabled = true;
    public string triggerTag;

    [Header("Dialog-Specific Properties")]
    public bool dialogPersists = false;
    [Space(10)]
    public bool lookAtMyTarget = false;
    public Transform myTarget = null;
    [Space(10)]
    public string[] dialogLines = new string[] { };
    public DialogManager.Speed dialogSpeed;

    public Dialog dialog;

    Collider triggerCollider;
    Light l;

    void Awake()
    {
        interactManager = FindObjectOfType<InteractManager>();
    }

    void Start()
    {
        triggerCollider = GetComponent<Collider>();
        l = transform.GetChild(0).gameObject.GetComponent<Light>();

        dialog = new Dialog();
        dialog.lines = dialogLines;
        dialog.speed = dialogSpeed;

        if (triggerEnabled) Enable();
        else Disable();
    }

    public string GetTag()
    {
        return triggerTag;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Enable()
    {
        triggerCollider.enabled = true;
        l.enabled = true;
    }

    public void Disable()
    {
        triggerCollider.enabled = false;
        l.enabled = false;
    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            interactManager.DialogEnterRange(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            interactManager.DialogExitRange(this);
        }
    }
}
