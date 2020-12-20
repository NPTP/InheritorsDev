using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    Transform playerTransform;
    Transform target;
    public float speed = 80f;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        target = playerTransform;
    }

    void Update()
    {
        LookAtTarget(target);
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

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ResetTarget()
    {
        target = playerTransform;
    }
}
