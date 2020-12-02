using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TriggerProjector : MonoBehaviour
{
    Projector projector;
    float defaultOrthoSize;
    float speed = 12.5f;
    bool projectorEnabled;

    void Awake()
    {
        projector = GetComponent<Projector>();
        defaultOrthoSize = projector.orthographicSize;
    }

    void Update()
    {
        if (projectorEnabled)
            transform.Rotate(0, 0, Time.deltaTime * speed, Space.Self);
    }

    public void Enable()
    {
        StopCoroutine("DisableAnimation");
        projector.enabled = true;
        projectorEnabled = true;
        DOTween.To(
            () => projector.orthographicSize,
            x => projector.orthographicSize = x,
            defaultOrthoSize,
            0.25f
        ).SetEase(Ease.OutBack);
    }

    public void Disable()
    {
        StartCoroutine("DisableAnimation");
    }

    IEnumerator DisableAnimation()
    {
        yield return null;
        Tween t = DOTween.To(
            () => projector.orthographicSize,
            x => projector.orthographicSize = x,
            0f,
            0.25f
        ).From(projector.orthographicSize);
        yield return t.WaitForCompletion();
        projector.enabled = false;
        projectorEnabled = false;
    }
}
