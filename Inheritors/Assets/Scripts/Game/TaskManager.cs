using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public class Task
    {
        public string label;        // Identifier for the task, e.g. "GetWood"
        public string originalText; // The actual text to display for the task, e.g. "Get wood for the fire."
        public string currentText;
        public bool active;
        public bool completed;
        public bool failed;
        string completedColorTag = "<color=green>"; // "<color=#006400>";
        string activeColorTag = "<color=#0000ff>";
        string inactiveColorTag = "<color=#808080>";
        string failedColorTag = "<color=#808080>";
        string endColorTag = "</color>";
        string lineBreak = "\n";

        public Task(string label = "", string originalText = "")
        {
            this.label = label;
            this.originalText = originalText;
            this.completed = false;
            Inactive();
        }

        public void Inactive()
        {
            this.currentText = inactiveColorTag + this.originalText + endColorTag + lineBreak;
            this.active = false;
            this.completed = false;
        }

        public void Active()
        {
            this.currentText = activeColorTag + "<b>" + this.originalText + "</b>" + endColorTag + lineBreak;
            this.active = true;
            this.completed = false;
        }

        public void Complete()
        {
            // string strikethroughText = StrikeThrough(this.originalText);
            // this.currentText = completedColorTag + strikethroughText + endColorTag + lineBreak;
            this.currentText = completedColorTag + originalText + endColorTag + lineBreak;
            this.active = false;
            this.completed = true;
        }

        public void Fail()
        {
            string strikethroughText = StrikeThrough(this.originalText);
            this.currentText = failedColorTag + strikethroughText + endColorTag + lineBreak;
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

    /* TaskManager properties */
    UIManager uiManager;
    List<Task> taskList;
    Task activeTask;
    public bool allTasksCompleted = false;

    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        taskList = new List<Task>();
        activeTask = new Task();
        ResetAllTasks();
    }

    public void AddTask(string label, string taskText)
    {
        Task newTask = new Task(label, taskText);
        taskList.Add(newTask);
        UpdateTasks();
    }

    public void SetActiveTask(string label)
    {
        Task newActiveTask = new Task();
        int index = 0;
        for (int i = 0; i < taskList.Count; i++)
        {
            if (taskList[i].label == label)
            {
                index = i;
                newActiveTask = taskList[i];
                break;
            }
        }
        newActiveTask.Active();
        activeTask = newActiveTask;
        taskList.RemoveAt(index);

        UpdateTasks();
    }

    public void AddAndSetActive(string label, string taskText)
    {
        AddTask(label, taskText);
        SetActiveTask(label);
    }

    public void CompleteActiveTask()
    {
        activeTask.Complete();

        CheckAllTasksCompleted();
        UpdateTasks();
    }

    private void CheckAllTasksCompleted()
    {
        if (activeTask.completed && taskList.Count == 0)
        {
            Debug.Log("All tasks completed!"); // TODO: remove debug logs
            allTasksCompleted = true;
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
