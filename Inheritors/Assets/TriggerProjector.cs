using UnityEngine;

public class TriggerProjector : MonoBehaviour
{
    Projector projector;
    float speed = 12.5f;
    bool projectorEnabled;

    void Awake()
    {
        projector = GetComponent<Projector>();
    }

    void Update()
    {
        if (projectorEnabled)
            transform.Rotate(0, 0, Time.deltaTime * speed, Space.Self);
    }

    // TODO: nice fades in/out for the projector enable/disable.
    public void Enable()
    {
        projector.enabled = true;
        projectorEnabled = true;
    }

    public void Disable()
    {
        projector.enabled = false;
        projectorEnabled = false;
    }
}
