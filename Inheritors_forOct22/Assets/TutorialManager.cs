using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    SceneTransitions sceneTransitions;
    TaskManager taskManager;

    int debugTasks = 1; // Have to start at one because we subtract 1 in the task manager

    void Start()
    {
        sceneTransitions = GameObject.Find("SceneTransitions").GetComponent<SceneTransitions>();
        taskManager = GameObject.Find("TaskManager").GetComponent<TaskManager>();

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
            sceneTransitions.LoadSceneByIndex(0); // Go back to menu

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
}
