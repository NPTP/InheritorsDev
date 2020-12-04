using UnityEngine;

public class DialogTrigger : MonoBehaviour, Trigger
{
    InteractManager interactManager;

    public bool triggerEnabled = true;
    public string triggerTag;
    bool destroyed = false;

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
        return triggerEnabled;
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
            projectorTransform.position = myTarget.position;
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
        if (!destroyed)
        {
            triggerCollider.enabled = false;
            l.enabled = false;
            triggerProjector.Disable();
        }
    }

    void OnDestroy()
    {
        destroyed = true;
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

}
