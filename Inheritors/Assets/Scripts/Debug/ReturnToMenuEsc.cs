using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuEsc : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
}
