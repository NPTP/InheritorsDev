using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuEsc : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Back"))
        {
            SceneManager.LoadScene(0);
        }
    }
}
