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
    }

    public void SetColor(Color color)
    {
        transition.color = color;
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

    public void Show(float showTime = 0f)
    {
        transition.enabled = true;
        canvasGroup.enabled = true;
        canvasGroup.DOFade(1f, showTime);
    }

    public void Hide(float hideTime = 0f)
    {
        StartCoroutine(Out(hideTime));
    }

    IEnumerator Out(float hideTime)
    {
        Tween t = canvasGroup.DOFade(0f, hideTime);
        yield return new WaitWhile(() => t != null && t.IsPlaying());
        transition.enabled = false;
        canvasGroup.enabled = false;
    }

}
