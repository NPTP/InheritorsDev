using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionManager : MonoBehaviour
{
    Image transition;
    CanvasGroup canvasGroup;

    void Start()
    {
        GameObject go = GameObject.Find("Transition");
        transition = go.GetComponent<Image>();
        canvasGroup = go.GetComponent<CanvasGroup>();
        Debug.Log(canvasGroup);
    }

    public void SetAlpha(float alpha)
    {
        Debug.Log(canvasGroup);
        canvasGroup.alpha = alpha;
    }

    public void FadeTo(float alpha, float duration, Ease ease = Ease.Linear)
    {
        canvasGroup.DOFade(alpha, duration).SetEase(ease);
    }
}
