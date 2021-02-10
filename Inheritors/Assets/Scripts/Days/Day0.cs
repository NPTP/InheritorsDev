using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DialogContent))]
public class Day0 : MonoBehaviour
{
    int dayNumber = 0;
    public bool enableDayScripts = true;

    Dictionary<string, Trigger> triggers = new Dictionary<string, Trigger>();

    /* -------------------------------------- */
    /* Day-specific objects, transforms, etc. */
    /* -------------------------------------- */

    [Header("Day-specific Objects")]
    public Transform firepitTransform;
    public Transform firewoodTransform;
    public Transform malocaMotherTransform;

    [Space]
    [Header("Audio sources")]
    public AudioSource fireAudio2D;
    public GameObject firepitAudio;

    [Space]
    [Header("Audio clips")]
    public float tutSoundVolumeScale = .5f;
    public AudioClip[] tutorialSounds;

    /* -------------------------------------- */
    /* -------------------------------------- */

    void OnEnable()
    {
        print("set playerprefs in day");
        PlayerPrefs.SetInt("currentDayNumber", dayNumber);
    }

    void Awake()
    {
        if (!enableDayScripts && Application.isEditor)
            Destroy(this);
        InitializeReferences();
    }

    void Start()
    {
        InitializeTriggers();
        SubscribeToEvents();
        StartCoroutine("Intro");
    }

    IEnumerator Intro()
    {
        PlayerPrefs.SetInt("CompletedGame", 0);
        firepitAudio.SetActive(false);

        stateManager.SetState(State.Inert);
        cameraManager.SwitchToCam("IntroCam");

        /* 01. Darken screen, wait. */
        transitionManager.SetColor(Color.black);
        transitionManager.Show();
        Tween transition = transitionManager.Hide(8f);
        yield return null;
        stateManager.SetState(State.Inert);
        yield return transition.WaitForCompletion();

        // Extra 2 seconds for flavour.
        yield return new WaitForSeconds(2);

        /* 02. Kick off the intro narration dialog. */
        dialogManager.NewDialog(dialogContent.Get("Opening"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        /* 03. Fade away the blackness. */
        yield return new WaitForSeconds(2f);
        cameraManager.SendCamTo(firepitTransform);
        cameraManager.Zoom(0.375f, 1f);
        yield return new WaitWhile(cameraManager.IsSwitching);
        yield return new WaitForSeconds(2f);

        /* 04. Change view from fire to player. */
        fireAudio2D.DOFade(0f, 2f);
        cameraManager.SwitchToCam("Player");
        cameraManager.ResetZoom(2f);
        yield return new WaitWhile(cameraManager.IsSwitching);

        Destroy(fireAudio2D.gameObject);
        firepitAudio.SetActive(true);

        /* 05. Display tutorial controls. */
        uiManager.controls.Show();
        yield return new WaitForSeconds(.25f);

        /* 06. Let player control. When they move the joystick, fade out prompts. 
        ** Enable the firepite walk trigger. */
        stateManager.SetState(State.Normal);
        yield return new WaitUntil(inputManager.IsLeftJoystickMoving);
        uiManager.controls.Hide();
        triggers["Walk_Firepit"].Enable();
    }

    SaveManager saveManager;
    StateManager stateManager;
    TaskManager taskManager;
    DialogManager dialogManager;
    InputManager inputManager;
    AudioManager audioManager;
    TransitionManager transitionManager;
    CameraManager cameraManager;
    UIManager uiManager;
    InteractManager interactManager;
    PickupManager pickupManager;
    RecordManager recordManager;

    DialogContent dialogContent;

    void InitializeReferences()
    {
        dialogContent = GetComponent<DialogContent>();

        saveManager = FindObjectOfType<SaveManager>();
        audioManager = FindObjectOfType<AudioManager>();
        taskManager = FindObjectOfType<TaskManager>();
        dialogManager = FindObjectOfType<DialogManager>();
        stateManager = FindObjectOfType<StateManager>();
        transitionManager = FindObjectOfType<TransitionManager>();
        inputManager = FindObjectOfType<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        uiManager = FindObjectOfType<UIManager>();
        interactManager = FindObjectOfType<InteractManager>();
        interactManager = FindObjectOfType<InteractManager>();
        pickupManager = FindObjectOfType<PickupManager>();
        recordManager = FindObjectOfType<RecordManager>();
    }

    void InitializeTriggers()
    {
        IEnumerable<Trigger> worldTriggers = FindObjectsOfType<MonoBehaviour>().OfType<Trigger>();
        foreach (Trigger trigger in worldTriggers)
        {
            triggers[trigger.GetTag()] = trigger;
        }
    }

    void SubscribeToEvents()
    {
        interactManager.OnPickup += HandlePickupEvent;
        interactManager.OnDropoff += HandleDropoffEvent;
        interactManager.OnDialog += HandleDialogEvent;
        interactManager.OnWalk += HandleWalkEvent;
    }

    void OnDestroy()
    {
        interactManager.OnPickup -= HandlePickupEvent;
        interactManager.OnDropoff -= HandleDropoffEvent;
        interactManager.OnDialog -= HandleDialogEvent;
        interactManager.OnWalk -= HandleWalkEvent;
    }

    void HandlePickupEvent(object sender, InteractManager.PickupArgs args)
    {
        PickupManager.Inventory inventory = args.inventory;
        switch (inventory.itemType)
        {
            case ItemType.Wood:
                if (inventory.itemQuantity == 1)
                {
                    taskManager.SetActiveTask(TaskType.IntroFirewood);
                    dialogManager.NewDialog(dialogContent.Get("Wood1"));
                }
                else if (inventory.itemQuantity == 2)
                {
                    dialogManager.NewDialog(dialogContent.Get("Wood2"));
                }
                else if (inventory.itemQuantity == 3)
                {
                    StartCoroutine(AllWoodCollected());
                }
                break;

            case ItemType.Null:
                Debug.Log("NULL pickup event");
                break;

            default:
                Debug.Log("Interact Manager gave unknown PICKUP tag to Day " + dayNumber);
                break;
        }
    }

    IEnumerator AllWoodCollected()
    {
        dialogManager.NewDialog(dialogContent.Get("Wood3"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        taskManager.ChangeTask(TaskType.IntroFirewood, "Drop wood on fire.");

        triggers["Dropoff_Wood"].Enable();

        stateManager.SetState(State.Normal);
    }

    void HandleDialogEvent(object sender, InteractManager.DialogArgs args)
    {
        string tag = args.tag;

        switch (tag)
        {
            case "DUMMY":
                break;

            default:
                Debug.Log("Interact Manager gave unknown dialog tag " + tag + " to Day " + dayNumber);
                break;
        }
    }

    IEnumerator WaitDialogEnd()
    {
        yield return new WaitUntil(dialogManager.IsDialogFinished);
        print("Dialog done, m'boy!");
    }

    void HandleDropoffEvent(object sender, InteractManager.DropoffArgs args)
    {
        string tag = args.tag;

        switch (tag)
        {
            case "Dropoff_Wood":
                StartCoroutine(DropoffWood());
                break;

            default:
                Debug.Log("Interact Manager gave unknown DROPOFF tag to Day " + dayNumber);
                break;
        }
    }

    void HandleWalkEvent(object sender, InteractManager.WalkArgs args)
    {
        string tag = args.tag;

        switch (tag)
        {
            case "Walk_Firepit":
                StartCoroutine(Walk_Firepit());
                break;

            case "Walk_End":
                StartCoroutine(End());
                break;

            case "Walk_SisterSleep":
                dialogManager.NewDialog(dialogContent.Get("SisterSleep"));
                break;

            default:
                Debug.Log("Interact Manager gave unknown WALK tag to Day " + dayNumber);
                break;
        }
    }

    IEnumerator Walk_Firepit()
    {
        dialogManager.NewDialog(dialogContent.Get("Firepit"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        cameraManager.SendCamTo(firewoodTransform);
        // yield return new WaitForSeconds(1f);
        yield return new WaitWhile(cameraManager.IsSwitching);

        triggers["Pickup_Wood1"].Enable();
        audioManager.PlayOneShot(tutorialSounds[0], tutSoundVolumeScale);
        yield return new WaitForSeconds(1f);
        triggers["Pickup_Wood2"].Enable();
        audioManager.PlayOneShot(tutorialSounds[1], tutSoundVolumeScale);
        yield return new WaitForSeconds(1f);
        triggers["Pickup_Wood3"].Enable();
        audioManager.PlayOneShot(tutorialSounds[2], tutSoundVolumeScale);
        yield return new WaitForSeconds(1f);

        cameraManager.SwitchToLastCam();
        uiManager.SetUpTasksInventory();
        // yield return new WaitForSeconds(0.5f);
        yield return new WaitWhile(cameraManager.IsSwitching);
        taskManager.AddTask(TaskType.IntroFirewood, "Fetch 3 logs for firewood.");
        taskManager.AddTask(TaskType.IntroMaloca, "Return to Maloca to sleep.");
        taskManager.SetActiveTask(TaskType.IntroFirewood);

        stateManager.SetState(State.Normal);
    }

    IEnumerator DropoffWood()
    {
        taskManager.CompleteActiveTask();

        stateManager.SetState(State.Inert);
        yield return new WaitForSeconds(1f);

        dialogManager.NewDialog(dialogContent.Get("Maloca"), State.Inert);
        yield return new WaitUntil(dialogManager.IsDialogFinished);

        yield return new WaitForSeconds(.25f);
        taskManager.SetActiveTask(TaskType.IntroMaloca, false);
        triggers["Walk_End"].Enable();
        audioManager.PlayOneShot(tutorialSounds[3], tutSoundVolumeScale);

        stateManager.SetState(State.Normal);
    }

    IEnumerator End()
    {

        stateManager.SetState(State.Inert);
        uiManager.TearDownTasksInventory();
        transitionManager.SetColor(Color.black);
        Tween t = transitionManager.Show(2f);
        audioManager.FadeOtherSources("Down", 2f);
        yield return t.WaitForCompletion();

        saveManager.SaveGame(dayNumber);
        Helper.LoadScene("Loading");
    }
}
