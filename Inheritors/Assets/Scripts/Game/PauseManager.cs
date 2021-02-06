using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PauseManager : MonoBehaviour
{
    PlayerMovement playerMovement;
    InputManager inputManager;
    StateManager stateManager;
    UIManager uiManager;
    TransitionManager transitionManager;
    AudioManager audioManager;

    CanvasGroup allButtonsCG;
    CanvasGroup mainButtonsCG;
    CanvasGroup confirmButtonsCG;
    GameObject lastSelectedButton;

    // Confirm actions
    int confirmAction;
    const int RESTART = 0;
    const int QUIT = 1;
    const string quitObjectName = "ReturningFromGame";

    float transitionFadeTime = 0.5f;

    State savedState;

    [SerializeField] AudioClip buttonSound;
    float buttonSoundVolumeScale = .3f;

    void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        inputManager = FindObjectOfType<InputManager>();
        stateManager = FindObjectOfType<StateManager>();
        uiManager = FindObjectOfType<UIManager>();
        transitionManager = FindObjectOfType<TransitionManager>();
        audioManager = FindObjectOfType<AudioManager>();

        allButtonsCG = GameObject.Find("PauseMenu").GetComponent<CanvasGroup>();
        mainButtonsCG = GameObject.Find("MainButtons").GetComponent<CanvasGroup>();
        confirmButtonsCG = GameObject.Find("ConfirmWindow").GetComponent<CanvasGroup>();
    }

    void Start()
    {
        allButtonsCG.interactable = false;
        mainButtonsCG.interactable = false;
        confirmButtonsCG.interactable = false;

        uiManager.pauseMenu.Deactivate();

        inputManager.OnButtonDown += HandleButtonDown;
    }

    void OnDestroy()
    {
        inputManager.OnButtonDown -= HandleButtonDown;
    }

    // Catch pause button during gameplay.
    void HandleButtonDown(object sender, InputManager.ButtonArgs args)
    {
        if (args.buttonCode == InputManager.START)
        {
            if (!allButtonsCG.gameObject.activeInHierarchy)
            {
                StartCoroutine(PauseMenuOpen());
            }
        }
    }

    IEnumerator PauseMenuOpen()
    {
        // Wait a frame in case other state has been engaged on the same frame as pressing START
        yield return null;

        // Now check the state, and break out of the coroutine if it's no good.
        State state = stateManager.GetState();
        if (state != State.Normal && state != State.Holding) { yield break; }

        savedState = stateManager.GetState();
        stateManager.SetState(State.Inert);
        playerMovement.Halt();
        yield return uiManager.Pause().WaitForCompletion();
        // Time.timeScale = 0;
        allButtonsCG.interactable = true;
        mainButtonsCG.interactable = true;
        confirmButtonsCG.interactable = false;
        EventSystem.current.SetSelectedGameObject(uiManager.pauseMenu.defaultSelectedButton);
    }

    public void Resume()
    {
        PlayButtonSound();
        StartCoroutine(PauseMenuClose());
    }

    IEnumerator PauseMenuClose()
    {
        allButtonsCG.interactable = false;
        // Time.timeScale = 1;
        yield return uiManager.Unpause().WaitForCompletion();
        stateManager.SetState(savedState);
        yield return null;
    }

    public void TryButton(string action)
    {
        string message = "";
        if (action.ToLower() == "restart")
        {
            confirmAction = RESTART;
            message = "Are you sure you want to restart?";
            lastSelectedButton = uiManager.pauseMenu.restartButton;
        }
        else if (action.ToLower() == "quit")
        {
            confirmAction = QUIT;
            message = "Are you sure you want to quit?";
            lastSelectedButton = uiManager.pauseMenu.quitButton;
        }

        PlayButtonSound();
        mainButtonsCG.interactable = false;
        Tween t = uiManager.PauseConfirmationUp(message);
        t.OnComplete(() =>
            {
                confirmButtonsCG.interactable = true;
                EventSystem.current.SetSelectedGameObject(uiManager.pauseMenu.confirmNoButton);
            }
        );
    }

    public void ConfirmYes()
    {
        if (confirmAction == RESTART)
            RestartDay();
        else if (confirmAction == QUIT)
            Quit();
    }

    public void ConfirmNo()
    {
        PlayButtonSound();
        confirmButtonsCG.interactable = false;
        Tween t = uiManager.PauseConfirmationDown();
        t.OnComplete(() =>
            {
                mainButtonsCG.interactable = true;
                EventSystem.current.SetSelectedGameObject(lastSelectedButton);
            }
        );
    }

    public void RestartDay()
    {
        PlayButtonSound();
        StartCoroutine(RestartDayCoroutine());
    }

    IEnumerator RestartDayCoroutine()
    {
        allButtonsCG.interactable = false;
        // Time.timeScale = 1;
        transitionManager.SetColor(Color.black);
        audioManager.FadeOtherSources("Down", transitionFadeTime);
        Tween t = transitionManager.Show(transitionFadeTime);
        yield return t.WaitForCompletion();
        Helper.RestartScene();
    }

    public void Quit()
    {
        PlayButtonSound();
        GameObject.Instantiate(Resources.Load(quitObjectName));
        StartCoroutine(QuitCoroutine());
    }

    IEnumerator QuitCoroutine()
    {
        allButtonsCG.interactable = false;
        // Time.timeScale = 1;
        transitionManager.SetColor(Color.black);
        audioManager.FadeOtherSources("Down", transitionFadeTime);
        Tween t = transitionManager.Show(transitionFadeTime);
        yield return t.WaitForCompletion();
        Helper.LoadScene("MainMenu");
    }

    void PlayButtonSound()
    {
        audioManager.PlayOneShot(buttonSound, buttonSoundVolumeScale);
    }

}
