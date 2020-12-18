using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    Transform player;
    float speed = 40f;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        LookAtTarget(player);
    }

    void LookAtTarget(Transform target)
    {
        Vector3 lookRotation = Quaternion.LookRotation(
                target.position - transform.position,
                Vector3.up).eulerAngles;
        lookRotation.x = 0f;
        lookRotation.z = 0f;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(lookRotation);
        transform.rotation = Quaternion.RotateTowards(startRot, endRot, Time.deltaTime * speed);
    }
}
