using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Flute_DropoffTarget : MonoBehaviour, DropoffTarget
{
    bool doneReaction = false;
    Renderer thisRenderer;
    float scaleTime = 0.25f;

    public bool DoneReaction()
    {
        return doneReaction;
    }

    void Awake()
    {
        thisRenderer = GetComponent<Renderer>();
        thisRenderer.enabled = false;
    }

    public void ReactToDropoff()
    {
        StartCoroutine(DropoffAnimation());
    }

    IEnumerator DropoffAnimation()
    {
        thisRenderer.enabled = true;
        Tween t = transform.DOScale(transform.localScale, scaleTime).From(Vector3.zero);
        yield return t.WaitForCompletion();

        doneReaction = true;

        yield return null;
        Destroy(this);
    }
}
