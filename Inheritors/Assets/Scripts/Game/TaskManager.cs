/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType
{
    Null,
    IntroFirewood,
    IntroMaloca,
    Mother,
    Father,
    Sister,
    Grandmother,
    Grandfather,
    DayEnd
}

public enum TaskStatus
{
    Disabled,
    Waiting,
    Active,
    Completed
}

public class Task
{
    public TaskType type;        // Identifier for the task
    public Character character; // The most likely character to be connected to this task
    public string text;         // The actual text to display for the task, e.g. "Get wood for the fire."
    public AreaTrigger area;
    public TaskStatus status;

    public Task(TaskType type = TaskType.Null, string text = "", AreaTrigger area = null)
    {
        this.type = type;
        AssignCharacter(type);
        this.text = text;
        this.area = area;
        this.status = TaskStatus.Disabled;
    }

    void AssignCharacter(TaskType taskType)
    {
        switch (taskType)
        {
            case TaskType.IntroFirewood:
            case TaskType.IntroMaloca:
            case TaskType.Mother:
            case TaskType.DayEnd:
                this.character = Character.Mother;
                break;
            case TaskType.Father:
                this.character = Character.Father;
                break;
            case TaskType.Sister:
                this.character = Character.Sister;
                break;
            case TaskType.Grandmother:
                this.character = Character.Grandmother;
                break;
            case TaskType.Grandfather:
                this.character = Character.Grandfather;
                break;
            case TaskType.Null:
            default:
                this.character = Character.Null;
                break;
        }
    }
}

public class TaskManager : MonoBehaviour
{
    UIManager uiManager;
    RecordManager recordManager;
    Task activeTask;
    bool allTasksCompleted = false;
    Dictionary<TaskType, AreaTrigger> areas = new Dictionary<TaskType, AreaTrigger>();
    Dictionary<TaskType, Task> taskList = new Dictionary<TaskType, Task>();

    public event EventHandler<EventArgs> OnAllTasks;
    public event EventHandler<TaskArgs> OnUpdateTasks;
    public class TaskArgs : EventArgs
    {
        public Task activeTask;
        public Dictionary<TaskType, Task> taskList;
    }

    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        recordManager = FindObjectOfType<RecordManager>();
        taskList = new Dictionary<TaskType, Task>();
        activeTask = new Task();
        InitializeTaskList();
        InitializeAreas();
    }

    void Start()
    {
        UpdateTasks();
    }

    void InitializeTaskList()
    {
        foreach (TaskType taskType in Enum.GetValues(typeof(TaskType)))
        {
            taskList[taskType] = new Task(taskType);
        }
        uiManager.UpdateTasks(activeTask, taskList);
    }

    void InitializeAreas()
    {
        AreaTrigger[] worldAreas = FindObjectsOfType<AreaTrigger>();
        foreach (AreaTrigger areaTrigger in worldAreas)
            areas[areaTrigger.taskType] = areaTrigger;
    }

    public Task GetTask(TaskType taskType)
    {
        return taskList[taskType];
    }

    public TaskStatus GetStatus(TaskType taskType)
    {
        return taskList[taskType].status;
    }

    public void AddTask(TaskType taskType, string taskText)
    {
        Task thisTask = taskList[taskType];
        thisTask.text = taskText;
        thisTask.status = TaskStatus.Waiting;

        if (areas.ContainsKey(taskType))
            thisTask.area = areas[taskType];
        else
            thisTask.area = null;

        UpdateTasks();
    }

    public void ChangeTask(TaskType taskType, string newText)
    {
        taskList[taskType].text = newText;
        UpdateTasks(false);
    }

    public void SetActiveTask(TaskType taskType, bool startRecording = true)
    {
        foreach (Task task in taskList.Values)
        {
            if (task.status == TaskStatus.Active)
            {
                print("A task is already active.");
                return;
            }
        }

        foreach (AreaTrigger area in areas.Values)
        {
            if (area.taskType == taskType)
                area.Remove();
            else
                area.Disable();
        }

        activeTask = taskList[taskType];
        activeTask.status = TaskStatus.Active;

        if (startRecording) { recordManager.StartNewRecording(); }
        UpdateTasks();
    }

    public void AddAndSetActive(TaskType taskType, string taskText, bool startRecording = true)
    {
        AddTask(taskType, taskText);
        SetActiveTask(taskType, startRecording);
    }

    public void CompleteActiveTask()
    {
        recordManager.StopRecording();

        foreach (AreaTrigger area in areas.Values)
        {
            area.Enable();
        }

        activeTask.status = TaskStatus.Completed;
        CheckAllTasksCompleted();

        activeTask = new Task(); // Null TaskType.
        UpdateTasks();
    }

    /// <summary>
    /// Used for days 8 and 9, where walk triggers alone complete a task,
    /// and would otherwise interrupt other active tasks that actually require fetching something.
    /// </summary>
    public void CompleteWaitingTask(TaskType taskType)
    {
        // recordManager.StopRecording();

        taskList[taskType].status = TaskStatus.Completed;
        CheckAllTasksCompleted();
        UpdateTasks();
    }

    // Just checks if any Waiting or Active tasks remain.
    // If not, then it's only Completed and Disabled tasks, so we must be all done.
    private void CheckAllTasksCompleted()
    {
        foreach (Task task in taskList.Values)
        {
            if (task.status == TaskStatus.Waiting || task.status == TaskStatus.Active)
            {
                return;
            }
        }

        allTasksCompleted = true;
        OnAllTasks?.Invoke(this, EventArgs.Empty);
    }

    void UpdateTasks(bool sendEvent = true)
    {
        if (sendEvent)
        {
            OnUpdateTasks?.Invoke(this, new TaskArgs
            {
                activeTask = this.activeTask,
                taskList = this.taskList
            });
        }
        uiManager.UpdateTasks(activeTask, taskList);
    }
}

