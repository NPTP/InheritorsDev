using UnityEngine;

public class Ghost : MonoBehaviour
{
    SampleBuffer sampleBuffer;
    [SerializeField] Animator animator;
    int frame = 0;

    void FixedUpdate()
    {
        if (frame < sampleBuffer.length)
        {
            Playback();
            frame++;
        }
        else
        {
            Destroy(this.gameObject);
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
    }
}
