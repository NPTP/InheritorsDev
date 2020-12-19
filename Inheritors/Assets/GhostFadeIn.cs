using UnityEngine;
using DG.Tweening;

public class GhostFadeIn : MonoBehaviour
{
    Renderer rend;
    float fadeInTime = 4f;
    public Renderer heldObjectRenderer = null;

    void Start()
    {
        rend = transform.GetChild(0).gameObject.GetComponent<Renderer>();
        rend.enabled = false;
        if (heldObjectRenderer != null)
        {
            heldObjectRenderer.enabled = false;
        }
    }

    public void FadeIn()
    {
        rend.enabled = true;

        for (int i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].SetFloat("_Globalopacity", 0);
            int mat = i;
            DOTween.To(
                () => rend.materials[mat].GetFloat("_Globalopacity"),
                x => rend.materials[mat].SetFloat("_Globalopacity", x),
                1,
                fadeInTime
            ).SetEase(Ease.InCubic);
        }

        if (heldObjectRenderer != null)
        {
            heldObjectRenderer.enabled = true;

            heldObjectRenderer.material.SetFloat("_Globalopacity", 0);
            DOTween.To(
                () => heldObjectRenderer.material.GetFloat("_Globalopacity"),
                x => heldObjectRenderer.material.SetFloat("_Globalopacity", x),
                1,
                fadeInTime
            ).SetEase(Ease.InCubic);
        }

        // for (int i = 0; i < rend.materials.Length; i++)
        // {
        //     heldObjectRenderer.materials[i].SetFloat("_Globalopacity", 0);
        //     int mat = i;
        //     DOTween.To(
        //         () => heldObjectRenderer.materials[mat].GetFloat("_Globalopacity"),
        //         x => heldObjectRenderer.materials[mat].SetFloat("_Globalopacity", x),
        //         1,
        //         fadeInTime
        //     ).SetEase(Ease.InCubic);
        // }
    }
}
