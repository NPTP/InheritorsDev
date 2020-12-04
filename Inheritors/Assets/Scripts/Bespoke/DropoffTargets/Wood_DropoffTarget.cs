using UnityEngine;
using DG.Tweening;

public class Wood_DropoffTarget : MonoBehaviour, DropoffTarget
{
    bool doneReaction = false;
    Renderer woodRenderer;

    public bool DoneReaction()
    {
        return doneReaction;
    }

    void Start()
    {
        woodRenderer = transform.GetChild(0).gameObject.GetComponent<Renderer>();
        woodRenderer.enabled = false;
    }

    public void ReactToDropoff()
    {
        woodRenderer.enabled = true;
        woodRenderer.transform.DOScale(woodRenderer.transform.localScale, .25f).From(Vector3.zero);
        doneReaction = true;
    }
}
