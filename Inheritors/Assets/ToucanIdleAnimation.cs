using System.Collections;
using UnityEngine;

public class ToucanIdleAnimation : MonoBehaviour
{
    Animation anim;
    string[] animations;

    void Start()
    {
        anim = GetComponent<Animation>();
        animations = new string[] {
            "toucan_idle2_noloop", "toucan_idle2_noloop",
            "toucan_idle2_noloop", "toucan_idle2_noloop",
            "toucan_idle2_noloop", "toucan_idle_noloop"
        };
        StartCoroutine(IdleAnims());
    }

    IEnumerator IdleAnims()
    {
        int i = 0;
        while (true)
        {
            if (i == animations.Length) { i = 0; }
            anim.Play(animations[i]);
            yield return new WaitWhile(() => anim.isPlaying);
            i++;
        }
    }
}
