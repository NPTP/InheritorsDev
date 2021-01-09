using System.Collections;

public abstract class MenuState
{
    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator Run()
    {
        yield break;
    }

    public virtual IEnumerator End()
    {
        yield break;
    }
}