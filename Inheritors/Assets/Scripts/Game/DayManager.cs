using System;
using UnityEngine;

// right now just acting as a state machine. Should be split off so that we have a
// separate state manager, and the day manager controls specific bespoke game events,
// task management, etc.
public class DayManager : MonoBehaviour
{
    public DayManager.State state;
    public enum State
    {
        Normal,
        Dialog,
        PickingUp,
        Holding,
        Inert
    }
    public event EventHandler<StateArgs> OnState;
    public class StateArgs : EventArgs
    {
        public DayManager.State state;
    }

    public event EventHandler<DialogArgs> OnDialog;
    public class DialogArgs : EventArgs
    {
        public string[] lines;
        public float speed;
    }

    public int dayNumber = 0; // We'll get this from the data for the day?
    // SceneLoader sceneLoader;
    InputManager inputManager;
    InteractManager interactManager;
    DialogManager dialogManager;
    TaskManager taskManager;

    int debugTasks = 1; // Have to start at one because we subtract 1 in the task manager

    void Start()
    {
        state = State.Normal;
        // sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        interactManager = GameObject.Find("InteractManager").GetComponent<InteractManager>();
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
    }

    void Update()
    {
        if (taskManager.allTasksCompleted)
            // sceneLoader.LoadSceneByIndex(0); // Go back to menu

            if (Input.GetKeyDown(KeyCode.C))
            {
                CompleteTask(debugTasks);
                debugTasks++;
            }
    }

    public void NewDialog(string[] lines, float speed)
    {
        OnDialog?.Invoke(this, new DialogArgs { lines = lines, speed = speed });
    }

    // This method is for the other managers to tell us what state we're in.
    public void SetState(DayManager.State newState)
    {
        this.state = newState;
        Debug.Log("State was just set to: " + this.state);
        OnState?.Invoke(this, new StateArgs { state = newState });
    }

    public DayManager.State GetState()
    {
        return this.state;
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
