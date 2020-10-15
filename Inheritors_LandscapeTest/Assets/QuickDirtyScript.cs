using UnityEngine;
using UnityEngine.UI;

public class QuickDirtyScript : MonoBehaviour
{
    RawImage rawImage;
    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
            rawImage.enabled = false;
        else
            rawImage.enabled = true;
    }
}
