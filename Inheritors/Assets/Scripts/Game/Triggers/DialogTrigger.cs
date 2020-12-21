using UnityEngine;

public class DialogTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public bool startEnabled = true;
    public string triggerTag;
    bool destroyed = false;
    bool disabled = false;

    Collider triggerCollider;
    Light l;
    TriggerProjector triggerProjector;
    Transform projectorTransform;

    [Header("Dialog-Specific Properties")]
    public Character character;
    [Space(10)]
    public bool dialogPersists = false;
    public bool dialogSkippable = false;
    [Space(10)]
    public bool lookAtMyTarget = false;
    public Transform myTarget = null;
    [Space(10)]

    public Dialog dialog;

    public bool StartedEnabled()
    {
        return startEnabled;
    }

    void Awake()
    {
        interactManager = FindObjectOfType<InteractManager>();
        projectorTransform = transform.GetChild(1);
        triggerProjector = projectorTransform.gameObject.GetComponent<TriggerProjector>();
        triggerCollider = GetComponent<Collider>();
        l = transform.GetChild(0).gameObject.GetComponent<Light>();

        dialog = new Dialog();
        dialog.character = character;
        dialog.lines = new string[] { "< NO DIALOG REFERENCE >" };
        dialog.skippable = dialogSkippable;

        if (myTarget != null)
        {
            projectorTransform.position = myTarget.position + new Vector3(0, 2f, 0);
            dialog.target = myTarget;
        }
    }

    void Start()
    {
        triggerProjector.Disable();

        if (startEnabled) Enable();
        else Disable();
    }

    public void DisableTriggerProjector()
    {
        triggerProjector.Disable();
    }

    public string GetTag()
    {
        return triggerTag;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void InteractAppear()
    {
        if (!disabled)
            EnableSteps();
    }

    public void InteractDisappear()
    {
        if (!disabled)
            DisableSteps();
    }


    public void Enable()
    {
        if (!destroyed)
        {
            EnableSteps();
            disabled = false;
        }
    }

    public void Disable()
    {
        if (!destroyed)
        {
            DisableSteps();
            disabled = true;
        }
    }

    // As in, the steps taken to enable
    void EnableSteps()
    {
        if (!destroyed)
        {
            triggerCollider.enabled = true;
            l.enabled = true;
        }
    }

    // As in, the steps taken to disable
    void DisableSteps()
    {
        if (!destroyed)
        {
            triggerCollider.enabled = false;
            l.enabled = false;
            triggerProjector.Disable();
        }
    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        destroyed = true;
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

}
