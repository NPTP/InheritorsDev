using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    StateManager stateManager;
    InputManager inputManager;

    [SerializeField] private float m_moveSpeed = 6;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;

    [SerializeField] private Animator m_animator = null;
    [SerializeField] private Rigidbody m_rigidBody = null;

    private float m_currentV = 0;
    private float m_currentH = 0;
    public Vector3 direction;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;
    private bool m_jumpInput = false;

    public bool m_isGrounded;
    private bool mobile = true;

    private List<Collider> m_collisions = new List<Collider>();

    private void Awake()
    {
        if (!m_animator) { gameObject.GetComponent<Animator>(); }
        if (!m_rigidBody) { gameObject.GetComponent<Animator>(); }
        inputManager = GameObject.FindObjectOfType<InputManager>();
        stateManager = GameObject.FindObjectOfType<StateManager>();

        direction = Vector3.zero;
    }

    public Sample GetSample()
    {
        Sample sample = new Sample();
        sample.isGrounded = m_isGrounded;
        sample.direction = direction;
        sample.position = transform.position;
        sample.rotation = transform.rotation;
        return sample;
    }

    private void FixedUpdate()
    {
        m_animator.SetBool("Grounded", m_isGrounded);

        State state = stateManager.GetState();
        if (state == State.Normal || state == State.PickingUp || state == State.Holding)
            DirectUpdate();

        m_wasGrounded = m_isGrounded;
        m_jumpInput = false;
    }

    private void DirectUpdate()
    {
        float v = inputManager.leftStickVertical;
        float h = inputManager.leftStickHorizontal;

        Transform camera = Camera.main.transform;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            v *= m_walkScale;
            h *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        // Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;
        direction = camera.forward * m_currentV + camera.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if (direction != Vector3.zero)
        {
            // Ensure we're in a good state to change rotation & position.
            if (stateManager.GetState() != State.Dialog || stateManager.GetState() != State.Inert)
            {
                m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);
                transform.rotation = Quaternion.LookRotation(m_currentDirection);
                transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;
            }
        }

        m_animator.SetFloat("MoveSpeed", direction.magnitude);
        // JumpingAndLanding();
    }

    public void Halt()
    {
        StartCoroutine(HaltProcess());
    }

    IEnumerator HaltProcess()
    {
        m_currentV = 0;
        m_currentH = 0;

        float elapsed = 0;
        float time = .25f;
        Vector3 savedDirection = direction;
        while (elapsed < time)
        {
            float t = Helper.SmoothStep(elapsed / time);
            direction = Vector3.Lerp(savedDirection, Vector3.zero, t);
            m_animator.SetFloat("MoveSpeed", direction.magnitude);
            yield return null;
            elapsed += Time.deltaTime;
        }
        direction = Vector3.zero;
        m_animator.SetFloat("MoveSpeed", direction.magnitude);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

    /*
    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && m_jumpInput)
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }
    */

}
