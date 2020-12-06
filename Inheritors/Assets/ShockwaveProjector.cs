using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ShockwaveProjector : MonoBehaviour
{
    Projector projector;
    float finalSize = 650f;
    float duration = 12f;

    void Awake()
    {
        projector = GetComponent<Projector>();
        projector.orthographicSize = 0;
        projector.enabled = false;
    }

    void Shockwave()
    {
        StartCoroutine(ShockwaveAnimation());
    }

    IEnumerator ShockwaveAnimation()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<ParticleSystem>().Play();
        }
        yield return new WaitForSeconds(0.25f);

        projector.enabled = true;
        Tween t = DOTween.To(
            () => projector.orthographicSize,
            x => projector.orthographicSize = x,
            finalSize,
            duration
        );

        yield return t.WaitForCompletion();
        projector.enabled = false;
        Destroy(this.gameObject);
    }
}
