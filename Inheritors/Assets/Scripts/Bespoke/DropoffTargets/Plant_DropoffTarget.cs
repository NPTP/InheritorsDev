using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Plant_DropoffTarget : MonoBehaviour, DropoffTarget
{
    bool doneReaction = false;

    Vector3 savedLocalScale;
    float growTime = .5f;

    public bool DoneReaction()
    {
        return doneReaction;
    }

    void Start()
    {
        savedLocalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    public void ReactToDropoff()
    {
        StartCoroutine(GrowAnimation());
    }

    IEnumerator GrowAnimation()
    {
        Tween t = transform.DOScale(savedLocalScale, growTime).SetEase(Ease.OutBack);
        yield return t.WaitForCompletion();
        doneReaction = true;
    }
}
