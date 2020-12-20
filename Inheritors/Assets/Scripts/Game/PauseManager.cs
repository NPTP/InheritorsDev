using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PauseManager : MonoBehaviour
{
    PlayerMovement playerMovement;
    InputManager inputManager;
    StateManager stateManager;
    PauseMenu pauseMenu;

    State savedState;

    void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        inputManager = FindObjectOfType<InputManager>();
        stateManager = FindObjectOfType<StateManager>();
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    void Start()
    {
        inputManager.OnButtonDown += HandleButtonDown;
        pauseMenu.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        inputManager.OnButtonDown -= HandleButtonDown;
    }

    void HandleButtonDown(object sender, InputManager.ButtonArgs args)
    {
        if (args.buttonCode == InputManager.START)
        {
            print("Start pressed");
            if (!pauseMenu.gameObject.activeInHierarchy)
            {
                savedState = stateManager.GetState();
                stateManager.SetState(State.Inert);
                playerMovement.Halt();
                pauseMenu.gameObject.SetActive(true);
                pauseMenu.Activate();
            }
        }
    }

    public void SetPauseMenuInactive()
    {
        stateManager.SetState(savedState);
        pauseMenu.gameObject.SetActive(false);
    }

}
