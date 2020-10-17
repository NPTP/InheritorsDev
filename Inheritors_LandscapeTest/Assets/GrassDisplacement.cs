using UnityEngine;

public class GrassDisplacement : MonoBehaviour
{
    public float playerForceRange = 1.5f;
    public float playerForceIntensity = 5f;
    private Transform playerT;
    private Transform otherT;
    private Renderer rdr;

    void Start()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
        otherT = GameObject.Find("Other").transform;
        rdr = GetComponent<Renderer>();

        // Set force range and intensities for player, from inspector
        rdr.material.SetFloat("_ForceRange", playerForceRange);
        rdr.material.SetFloat("_ForceIntensity", playerForceIntensity);

        // Set force range and intensities for other
        float otherScale = (otherT.localScale.x + otherT.localScale.z) / 2;
        rdr.material.SetFloat("_ForceRange2", otherScale);
        rdr.material.SetFloat("_ForceIntensity2", otherScale);
    }

    void Update()
    {
        // Update displacement for player
        Vector3 relativePoint = transform.InverseTransformPoint(playerT.position);
        rdr.material.SetVector("_ForceCenter", new Vector4(
            relativePoint.x,
            0f,
            relativePoint.z,
            0f
        ));

        // Update displacement for other
        Vector3 otherPoint = transform.InverseTransformPoint(otherT.position);
        rdr.material.SetVector("_ForceCenter2", new Vector4(
            otherPoint.x,
            0f,
            otherPoint.z,
            0f
        ));
    }
}
