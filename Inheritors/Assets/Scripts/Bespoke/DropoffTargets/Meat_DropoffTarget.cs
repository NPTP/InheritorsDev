using UnityEngine;
using DG.Tweening;

public class Meat_DropoffTarget : MonoBehaviour, DropoffTarget
{
    Renderer meatRenderer;

    void Start()
    {
        meatRenderer = transform.GetChild(0).gameObject.GetComponent<Renderer>();
        meatRenderer.enabled = false;
    }

    public void ReactToDropoff()
    {
        meatRenderer.enabled = true;
        meatRenderer.material.color = Helper.ChangedAlpha(meatRenderer.material.color, 0f);
        meatRenderer.transform.DOScale(meatRenderer.transform.localScale, .25f).From(Vector3.zero);
        meatRenderer.material.DOColor(
            Helper.ChangedAlpha(meatRenderer.material.color, 1f),
            .25f
        );
    }
}
