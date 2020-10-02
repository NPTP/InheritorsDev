using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwivelScript : MonoBehaviour
{
    private RewindManagerScript rewindManagerScript;
    private List<Vector3> rotationList;
    public float rotationSpeed = 25f;
    void Start()
    {
        GameObject rewindManager = GameObject.Find("RewindManager");
        rewindManagerScript = rewindManager.GetComponent<RewindManagerScript>();
        rotationList = new List<Vector3>();
    }

    void Update()
    {
        if (!rewindManagerScript.rewinding || rotationList.Count < 1)
        {
            if (transform.rotation.eulerAngles.y >= 360f)
            {
                transform.rotation.eulerAngles.Set(0f, 0f, 0f);
            }
            else
            {
                transform.Rotate(0.0f, rotationSpeed * Time.deltaTime, 0.0f, Space.Self);
            }

            rotationList.Add(transform.rotation.eulerAngles);
        }
        else
        {
            int lastIndex = rotationList.Count - 1;
            Vector3 position = rotationList[lastIndex];
            transform.rotation = Quaternion.Euler(position.x, position.y, position.z);          // World. use transform.localRotation for local
            rotationList.RemoveAt(lastIndex);
        }


    }
}
