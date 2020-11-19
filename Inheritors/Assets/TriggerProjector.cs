using UnityEngine;

public class TriggerProjector : MonoBehaviour
{
    Projector projector;
    float speed = 12.5f;

    void Start()
    {
        projector = GetComponent<Projector>();
    }

    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * speed, Space.Self);
    }

    void Enable()
    {
        
    }
}
