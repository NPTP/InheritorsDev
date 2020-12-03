using UnityEngine;
using DG.Tweening;

public class Water_DropoffTarget : MonoBehaviour, DropoffTarget
{
    Renderer waterRenderer;

    void Start()
    {
        waterRenderer = transform.GetChild(0).gameObject.GetComponent<Renderer>();
        waterRenderer.enabled = false;
    }

    public void ReactToDropoff()
    {
        waterRenderer.enabled = true;
        waterRenderer.material.color = Helper.ChangedAlpha(waterRenderer.material.color, 0f);
        waterRenderer.material.DOColor(
            Helper.ChangedAlpha(waterRenderer.material.color, 1f),
            .1f
        );
    }
}
