using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Water_DropoffTarget : MonoBehaviour, DropoffTarget
{
    Renderer waterRenderer;
    GameObject jug;

    bool doneReaction = false;
    public bool DoneReaction()
    {
        return doneReaction;
    }

    void Start()
    {
        waterRenderer = transform.GetChild(0).gameObject.GetComponent<Renderer>();
        waterRenderer.enabled = false;
        jug = transform.GetChild(1).gameObject;
        jug.SetActive(false);
    }

    public void ReactToDropoff()
    {
        StartCoroutine(DropoffAnimation());
    }

    IEnumerator DropoffAnimation()
    {
        waterRenderer.enabled = true;
        waterRenderer.material.color = Helper.ChangedAlpha(waterRenderer.material.color, 0f);

        jug.SetActive(true);
        Tween t = jug.transform.DOScale(jug.transform.localScale, .25f).From(Vector3.zero);
        yield return t.WaitForCompletion();

        t = waterRenderer.material.DOColor(Helper.ChangedAlpha(waterRenderer.material.color, 1f), 1f);
        yield return t.WaitForCompletion();

        t = jug.transform.DOScale(0f, .25f);
        yield return t.WaitForCompletion();

        doneReaction = true;

        yield return null;
        Destroy(jug);
        Destroy(this);
    }
}
