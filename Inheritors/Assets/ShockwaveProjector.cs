using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ShockwaveProjector : MonoBehaviour
{
    Projector projector;
    float finalSize = 450;
    float duration = 8.5f;
    bool finished = false;

    void Awake()
    {
        projector = GetComponent<Projector>();
        projector.orthographicSize = 0;
        projector.enabled = false;
    }

    public void Shockwave()
    {
        StartCoroutine(ShockwaveAnimation());
    }

    public bool ShockwaveFinished()
    {
        return finished;
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
        finished = true;

        yield return null;
        Destroy(this.gameObject);
    }
}
