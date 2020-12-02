/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

public class TaskManager : MonoBehaviour
{
    public class Task
    {
        public TaskType type;        // Identifier for the task
        public string text;         // The actual text to display for the task, e.g. "Get wood for the fire."
        public AreaTrigger area;
        public bool active;
        public bool completed;
        public bool failed;

        string completedColorTag = "<color=green>";
        string activeColorTag = "<color=#0000ff>";
        string inactiveColorTag = "<color=#808080>";
        string failedColorTag = "<color=#808080>";
        string endColorTag = "</color>";

        public Task(TaskType type = TaskType.Null, string text = "", AreaTrigger area = null)
        {
            this.type = type;
            this.text = text;
            this.area = area;
            this.completed = false;
            Inactive();
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

    // ████████████████████████████████████████████████████████████████████████
    // ███ TASKMANAGER PROPERTIES
    // ████████████████████████████████████████████████████████████████████████

    UIManager uiManager;
    RecordManager recordManager;
    List<Task> taskList;
    Task activeTask;
    bool allTasksCompleted = false;
    public event EventHandler<EventArgs> OnAllTasks;
    Dictionary<string, AreaTrigger> areas = new Dictionary<string, AreaTrigger>();

    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        recordManager = FindObjectOfType<RecordManager>();
        taskList = new List<Task>();
        activeTask = new Task();
        ResetAllTasks();
        InitializeAreas();
    }

    void InitializeAreas()
    {
        AreaTrigger[] worldAreas = FindObjectsOfType<AreaTrigger>();
        foreach (AreaTrigger areaTrigger in worldAreas)
            areas[areaTrigger.GetTag()] = areaTrigger;
    }

    public void AddTask(TaskType type, string taskText, AreaTrigger area = null)
    {
        Task newTask = new Task(type, taskText, area);
        taskList.Add(newTask);
        UpdateTasks();
    }

    public void ChangeTask(TaskType type, string newText)
    {
        if (activeTask.type == type)
        {
            activeTask.text = newText;
        }
        else
        {
            foreach (Task task in taskList)
            {
                if (task.type == type)
                {
                    task.text = newText;
                    break;
                }
            }
        }

        UpdateTasks();
    }

    public void SetActiveTask(TaskType type, bool startRecording = true)
    {
        if (activeTask.active)
        {
            print("A task is already active.");
            return;
        }

        Task newActiveTask = new Task();
        int index = 0;
        for (int i = 0; i < taskList.Count; i++)
        {
            if (taskList[i].type == type)
            {
                index = i;
                newActiveTask = taskList[i];
                break;
            }
        }
        if (newActiveTask.type == TaskType.Null)
        {
            print("No task with that type in active task or task list.");
            return;
        }

        if (newActiveTask.area != null)
        {
            newActiveTask.area.Disable();
            foreach (AreaTrigger area in areas.Values)
            {
                if (area != newActiveTask.area) { area.ShutDownArea(); }
            }
        }

        newActiveTask.Active();
        activeTask = newActiveTask;
        taskList.RemoveAt(index);

        if (startRecording) { recordManager.StartNewRecording(); }
        UpdateTasks();
    }

    public void AddAndSetActive(TaskType type, string taskText, bool startRecording = true, AreaTrigger area = null)
    {
        AddTask(type, taskText);
        SetActiveTask(type, startRecording);
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

        activeTask.Complete();

        activeTask = new Task();
        CheckAllTasksCompleted();
        UpdateTasks();
    }

    private void CheckAllTasksCompleted()
    {
        if (taskList.Count == 0)
        {
            Debug.Log("All tasks completed!");
            allTasksCompleted = true;
            OnAllTasks?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ResetAllTasks()
    {
        foreach (Task task in taskList)
            task.Reset();
        UpdateTasks();
    }

    public void ClearTasks()
    {
        activeTask = new Task();
        taskList.Clear();
        UpdateTasks();
    }

    void UpdateTasks()
    {
        uiManager.UpdateTasks(activeTask, taskList);
    }
}
