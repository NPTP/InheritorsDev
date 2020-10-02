using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector2 rotation = Vector2.zero;
    public float lookSpeed = 3;

    void Update()
    {
        rotation.x += -Input.GetAxis("Mouse Y");
        rotation.y += Input.GetAxis("Mouse X");
        rotation.x = Mathf.Clamp(rotation.x, -5f, 5f);
        rotation.y = Mathf.Clamp(rotation.y, -5f, 5f);
        transform.eulerAngles = rotation * lookSpeed;
        // Camera.main.transform.localRotation = Quaternion.Euler(rotation.x * lookSpeed, 0, 0);
    }
}
