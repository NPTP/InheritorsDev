using UnityEngine;

public class ProjectorRotate : MonoBehaviour
{
    float speed = 12.5f;

    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * speed, Space.Self);
    }
}
