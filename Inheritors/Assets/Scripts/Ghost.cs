using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Ghost : MonoBehaviour
{
    Renderer rend;
    SampleBuffer sampleBuffer;
    [SerializeField] Animator animator;
    [SerializeField] Texture dissolveMap;

    bool playing = false;
    int frame = 0;

    void FixedUpdate()
    {
        if (!playing) return;

        if (frame < sampleBuffer.length)
        {
            Playback();
            frame++;
        }
        else
        {
            playing = false;
            StartCoroutine(GhostExit());
        }
    }

    void Playback()
    {
        Sample sample = sampleBuffer.Get(frame);

        animator.SetBool("Grounded", sample.isGrounded);
        transform.position = sample.position;
        transform.rotation = sample.rotation;
        Vector3 direction = sample.direction;
        if (direction != Vector3.zero)
        {
            animator.SetFloat("MoveSpeed", direction.magnitude);
        }
    }

    public void PassBuffer(SampleBuffer buffer)
    {
        sampleBuffer = buffer;
        rend = transform.GetChild(1).gameObject.GetComponent<Renderer>();
        StartCoroutine(GhostEnter());
    }

    IEnumerator GhostEnter()
    {
        Tween t = null;

        for (int i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].shader = Shader.Find("Custom/ForcefieldDissolve");
            rend.materials[i].SetTexture("Dissolve Map", dissolveMap);
            rend.materials[i].SetFloat("_DissolveAmount", 1);

            int mat = i;
            t = DOTween.To(
                () => rend.materials[mat].GetFloat("_DissolveAmount"),
                x => rend.materials[mat].SetFloat("_DissolveAmount", x),
                0,
                2
            ).From(1);
        }

        yield return t.WaitForCompletion();

        for (int i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].shader = Shader.Find("Custom/Forcefield");
        }

        playing = true;
    }

    IEnumerator GhostExit()
    {
        Tween t = null;

        for (int i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i].shader = Shader.Find("Custom/ForcefieldDissolve");
            rend.materials[i].SetTexture("Dissolve Map", dissolveMap);
            rend.materials[i].SetFloat("_DissolveAmount", 0);

            int mat = i;
            t = DOTween.To(
                () => rend.materials[mat].GetFloat("_DissolveAmount"),
                x => rend.materials[mat].SetFloat("_DissolveAmount", x),
                1,
                2
            ).From(0);
        }

        yield return t.WaitForCompletion();

        Destroy(this.gameObject);
    }
}
