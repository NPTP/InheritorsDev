/* INHERITORS by Nick Perrin (c) 2020 */
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionManager : MonoBehaviour
{
    Image transition;
    CanvasGroup canvasGroup;

    void Awake()
    {
        GameObject go = GameObject.Find("Transition");
        transition = go.GetComponent<Image>();
        canvasGroup = go.GetComponent<CanvasGroup>();
    }

    public void SetColor(Color color)
    {
        transition.color = color;
    }

    public void ChangeColor(Color color, float duration)
    {
        transition.DOColor(color, duration);
    }

    public void SetAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
    }

    public void FadeTo(float alpha, float duration, Ease ease = Ease.Linear)
    {
        canvasGroup.DOFade(alpha, duration).SetEase(ease);
    }

    public Tween Show(float showTime = 0f)
    {
        transition.enabled = true;
        canvasGroup.enabled = true;
        return canvasGroup.DOFade(1f, showTime);
    }

    public Tween Hide(float hideTime = 0f)
    {
        Tween t = canvasGroup.DOFade(0f, hideTime);
        StartCoroutine(Out(t, hideTime));
        return t;
    }

    IEnumerator Out(Tween fadeTween, float hideTime)
    {
        yield return fadeTween.WaitForCompletion();
        transition.enabled = false;
        canvasGroup.enabled = false;
    }

}
