using System;
using UnityEngine;

// The central hub of our in-game logic. Apart from the main menu, the Day Manager is
// the ONLY class that should have a reference to all the other managers.
public class DayManager : MonoBehaviour
{
    public int dayNumber = 0;
    SceneLoader sceneLoader;
    InputManager inputManager;
    DialogManager dialogManager;
    TaskManager taskManager;

    int debugTasks = 1; // Have to start at one because we subtract 1 in the task manager

    void Start()
    {
        sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        dialogManager = GameObject.Find("DialogManager").GetComponent<DialogManager>();
        taskManager = GameObject.Find("TaskManager").GetComponent<TaskManager>();

        // TODO: Get the days tasks and data etc from a scriptable object

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
            sceneLoader.LoadSceneByIndex(0); // Go back to menu

        if (Input.GetKeyDown(KeyCode.C))
        {
            CompleteTask(debugTasks);
            debugTasks++;
        }
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
