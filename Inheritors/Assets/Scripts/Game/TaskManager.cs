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
    MotherWood,
    MotherWater,
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
    public string text;         // The actual text to display for the task, e.g. "Get wood for the fire."
    public AreaTrigger area;
    public TaskStatus status;

    public Task(TaskType type = TaskType.Null, string text = "", AreaTrigger area = null)
    {
        this.type = type;
        this.text = text;
        this.area = area;
        this.status = TaskStatus.Disabled;
    }
}

public class TaskManager : MonoBehaviour
{
    UIManager uiManager;
    RecordManager recordManager;
    Task activeTask;
    bool allTasksCompleted = false;
    public event EventHandler<EventArgs> OnAllTasks;
    Dictionary<TaskType, AreaTrigger> areas = new Dictionary<TaskType, AreaTrigger>();
    Dictionary<TaskType, Task> taskList = new Dictionary<TaskType, Task>();

    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        recordManager = FindObjectOfType<RecordManager>();
        taskList = new Dictionary<TaskType, Task>();
        activeTask = new Task();
        InitializeTaskList();
        InitializeAreas();
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

    Task GetTask(TaskType taskType)
    {
        return taskList[taskType];
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
        UpdateTasks();
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

        activeTask = taskList[taskType];
        activeTask.status = TaskStatus.Active;

        if (activeTask.area != null)
        {
            activeTask.area.Disable();
            foreach (AreaTrigger area in areas.Values)
            {
                if (area != activeTask.area) { area.ShutDownArea(); }
            }
        }

        if (startRecording) { recordManager.StartNewRecording(); }
        UpdateTasks();
    }

    public void AddAndSetActive(TaskType taskType, string taskText, bool startRecording = true, AreaTrigger area = null)
    {
        AddTask(taskType, taskText);
        SetActiveTask(taskType, startRecording);
    }

    public void CompleteActiveTask()
    {
        recordManager.StopRecording();

        if (activeTask.area != null)
        {
            activeTask.area.Remove();
            foreach (AreaTrigger area in areas.Values)
                if (area != null) { area.StartUpArea(); }
        }

        activeTask.status = TaskStatus.Completed;
        CheckAllTasksCompleted();
        UpdateTasks();

        activeTask = new Task();
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

    void UpdateTasks()
    {
        uiManager.UpdateTasks(activeTask, taskList);
    }

    // TODO: Remove this if unused.
    public class OldTask
    {
        public TaskType type;        // Identifier for the task
        public string text;         // The actual text to display for the task, e.g. "Get wood for the fire."
        public AreaTrigger area;
        public TaskStatus status;
        public bool waiting = true;
        public bool active = false;
        public bool completed = false;
        public bool failed;

        string completedColorTag = "<color=green>";
        string activeColorTag = "<color=#0000ff>";
        string inactiveColorTag = "<color=#808080>";
        string failedColorTag = "<color=#808080>";
        string endColorTag = "</color>";

        public OldTask(TaskType type = TaskType.Null, string text = "", AreaTrigger area = null)
        {
            this.type = type;
            this.text = text;
            this.area = area;
            this.status = TaskStatus.Waiting;
            // Inactive();
        }

        public void Inactive()
        {
            this.text = inactiveColorTag + this.text + endColorTag;
            this.active = false;
            this.completed = false;
        }

        public void Active()
        {
            this.text = activeColorTag + "<b>" + this.text + "</b>" + endColorTag; // + lineBreak;
            this.active = true;
            this.completed = false;
        }

        public void Complete()
        {
            // string strikethroughText = StrikeThrough(this.text);
            // this.text = completedColorTag + strikethroughText + endColorTag + doubleLineBreak;
            this.text = completedColorTag + text + endColorTag;
            this.active = false;
            this.completed = true;
        }

        public void Fail()
        {
            this.text = failedColorTag + StrikeThrough(this.text) + endColorTag;
            this.active = false;
            this.failed = true;
        }

        private string StrikeThrough(string s)
        {
            string strikethrough = "";
            foreach (char c in s)
            {
                strikethrough = strikethrough + c + '\u0336';
            }
            return strikethrough;
        }

        public void Reset()
        {
            Inactive();
        }
    }
}

