using UnityEngine;
using DG.Tweening;

public class Meat_DropoffTarget : MonoBehaviour, DropoffTarget
{
    bool doneReaction = false;
    GameObject roastingPig;
    Renderer meatRenderer;
    Vector3 savedScale;

    [SerializeField] ParticleSystem fireParticlesParent;

    public bool DoneReaction()
    {
        return doneReaction;
    }

    void Start()
    {
        roastingPig = GameObject.Find("RoastingPig");
        savedScale = roastingPig.transform.localScale;

        meatRenderer = GameObject.Find("roastingPigRender").GetComponent<Renderer>();
        meatRenderer.enabled = false;
    }

    public void ReactToDropoff()
    {
        meatRenderer.enabled = true;
        roastingPig.transform.DOScale(savedScale, .25f).From(Vector3.zero);
        fireParticlesParent.Stop();
        doneReaction = true;
    }
}
