using UnityEngine;

public class PauseMenuButtonCalls : MonoBehaviour
{
    PauseManager pauseManager;

    void Awake()
    {
        pauseManager = FindObjectOfType<PauseManager>();
    }

    public void Resume()
    {
        pauseManager.Resume();
    }

    public void RestartDay()
    {
        pauseManager.TryButton("Restart");
    }

    public void SaveAndQuit()
    {
        pauseManager.TryButton("Quit");
    }

    public void ConfirmYes()
    {
        pauseManager.ConfirmYes();
    }

    public void ConfirmNo()
    {
        pauseManager.ConfirmNo();
    }
}
