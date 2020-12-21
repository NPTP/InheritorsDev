using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Fish_DropoffTarget : MonoBehaviour, DropoffTarget
{
    bool doneReaction = false;

    public bool DoneReaction()
    {
        return doneReaction;
    }

    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void ReactToDropoff()
    {
        StartCoroutine(DropoffProcess());
    }

    IEnumerator DropoffProcess()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            child.DOScale(child.localScale, .25f).From(Vector3.zero);
            yield return new WaitForSeconds(.25f);
        }

        doneReaction = true;
    }
}
