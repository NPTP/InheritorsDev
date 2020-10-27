using System;
using UnityEngine;

// The central hub of our in-game logic. Apart from the main menu, the Day Manager is
// the ONLY class that should have a reference to all the other managers.
// It also handles the current "state".
public class DayManager : MonoBehaviour
{
    public int state;
    public enum State
    {
        Normal,
        Dialog,
        Pickup,
        Inert
    }
    public event EventHandler<StateArgs> OnState;
    public class StateArgs : EventArgs
    {
        public int state;
    }

    public event EventHandler<DialogArgs> OnDialog;
    public class DialogArgs : EventArgs
    {
        public string[] lines;
    }

    public int dayNumber = 0; // We'll get this from the data for the day?
    SceneLoader sceneLoader;
    InputManager inputManager;
    DialogManager dialogManager;
    TaskManager taskManager;

    int debugTasks = 1; // Have to start at one because we subtract 1 in the task manager

    void Start()
    {
        state = (int)State.Normal;
        sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        dialogManager = GameObject.Find("DialogManager").GetComponent<DialogManager>();
        taskManager = GameObject.Find("TaskManager").GetComponent<TaskManager>();

        // TODO: Get the day's tasks and data etc from a scriptable object or something like this,
        // to allow generalizing across day as much as possible. Then we just plug in the bespoke
        // content as we go, and it works.

        string[] tutorialTasks = {
            "Get wood for the fire",
            "Put wood on the fire",
            "Listen to mama's story"
        };

        foreach (string task in tutorialTasks)
            taskManager.AddTask(task);
        taskManager.SetActiveTask(1);

        Debug.Log("Press X to test dialog system. Note that this breaks InputManager for X, right now.");
    }

    void Update()
    {
        if (taskManager.allTasksCompleted)
            sceneLoader.LoadSceneByIndex(0); // Go back to menu

        if (Input.GetKeyDown(KeyCode.C))
        {
            CompleteTask(debugTasks);
            debugTasks++;
        }

        if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("X"))
        {
            EnterDialog();
        }
    }

    void EnterDialog()
    {
        SetState(State.Dialog);
        // TODO: have a way for the day or other object to pass this dialog in naturally, for now hardcoded to test
        string[] l = {
            "Lorem <b>ipsum</b> dolor sit amet, consectetur .  .  .  .  .  .  elit, sed do eiusmod tempor incididunt ut labore  .  .  .  .",
            "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
            "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
            "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
        };
        // TODO: send a dialog speed as well.
        OnDialog?.Invoke(this, new DialogArgs { lines = l });
    }

    public void SetState(DayManager.State newState)
    {
        this.state = (int)newState;
        OnState?.Invoke(this, new StateArgs { state = (int)newState });
    }

    void CompleteTask(int taskNum)
    {
        taskManager.CompleteTask(taskNum);
    }

    void BlockInput()
    {
        inputManager.BlockInput();
    }

    void AllowInput()
    {
        inputManager.AllowInput();
    }
}
