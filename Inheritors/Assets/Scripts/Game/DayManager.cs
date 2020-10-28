using System;
using UnityEngine;

// The central hub of our in-game logic. Apart from the main menu, the Day Manager is
// the ONLY class that should have a reference to all the other managers.
// It also handles the current "state".
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
        public int speed;
    }

    public int dayNumber = 0; // We'll get this from the data for the day?
    SceneLoader sceneLoader;
    InputManager inputManager;
    InteractManager interactManager;
    DialogManager dialogManager;
    TaskManager taskManager;

    int debugTasks = 1; // Have to start at one because we subtract 1 in the task manager

    void Start()
    {
        state = State.Normal;
        sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
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

        Debug.Log("Press Y (kbd/controller) to test dialog system. Note that this breaks all input on Y right now.");
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

        if (Input.GetKeyDown(KeyCode.Y) || Input.GetButtonDown("Y"))
        {
            EnterDialog();
        }
    }

    void EnterDialog()
    {
        string spacer = "                    ";
        if (state != State.Dialog)
        {
            // TODO: have a way for the day or other object to pass this dialog in naturally, for now hardcoded to test
            string[] l = {
                "The Omerê is our home." +spacer,
                "Our people have lived here for hundreds of years." +spacer,
                "Your mother is <b>Kanoê</b>." + spacer + "\nYour father is <b>Akuntsu</b>."  +spacer,
                "You are young, <b>Operaeika</b>." + spacer + "\nYou are the inheritor of this land." + spacer + "\nYou are the inheritor of our tradition."+spacer,
                "Bring us hope."  +spacer + spacer,
            };
            OnDialog?.Invoke(this, new DialogArgs { lines = l, speed = 1 });
        }
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
