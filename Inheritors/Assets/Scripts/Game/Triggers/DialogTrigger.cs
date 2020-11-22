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
    TriggerProjector triggerProjector;
    Transform projectorTransform;

    [Header("Dialog-Specific Properties")]
    public string speakerName = "Trigger Placeholder";
    [Space(10)]
    public bool dialogPersists = false;
    public bool dialogSkippable = false;
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
        projectorTransform = transform.GetChild(1);
        triggerProjector = projectorTransform.gameObject.GetComponent<TriggerProjector>();
        triggerCollider = GetComponent<Collider>();
        l = transform.GetChild(0).gameObject.GetComponent<Light>();
    }

    void Start()
    {
        dialog = new Dialog();
        dialog.name = speakerName;
        dialog.lines = dialogLines;
        dialog.speed = dialogSpeed;
        dialog.skippable = dialogSkippable;

        if (myTarget != null)
        {
            projectorTransform.transform.position = myTarget.position;
            dialog.target = myTarget;
        }
        triggerProjector.Disable();

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
            if (myTarget != null)
                triggerProjector.Enable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            interactManager.DialogExitRange(this);
            if (myTarget != null)
                triggerProjector.Disable();
        }
    }

    public void FlagInArea(AreaTrigger area) { }
}
