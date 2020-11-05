using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TestTween : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Thing());
    }

    IEnumerator Thing()
    {
        yield return new WaitForSeconds(1f);
        RectTransform rt = GetComponent<RectTransform>();
        Image i = GetComponent<Image>();
        rt.localScale = new Vector3(0f, 1f, 1f);
        Vector3 desiredScale = new Vector3(1f, 1f, 1f);

        i.DOFade(1f, 2f).From(0f);
        DOTween.To(
            () => rt.localScale,
            x => rt.localScale = x,
            desiredScale,
            .5f
        ).SetEase(Ease.OutCubic);
    }
}
