using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    private class Task
    {
        public int number;
        public string originalText;
        public string currentText;
        public bool active;
        public bool completed;
        string completedColorTag = "<color=#006400>";
        string activeColorTag = "<color=#0000ff>";
        string inactiveColorTag = "<color=#808080>";
        string endColorTag = "</color>";
        string lineBreak = "\n";

        public Task(int number, string originalText)
        {
            this.number = number;
            this.originalText = originalText;
            this.completed = false;
            Inactive();
        }

        public void Inactive()
        {
            this.currentText = inactiveColorTag + this.number.ToString() + ". " + this.originalText + endColorTag + lineBreak;
            this.active = false;
            this.completed = false;
        }

        public void Active()
        {
            this.currentText = activeColorTag + this.number.ToString() + ". " + "<b>" + this.originalText + "</b>" + endColorTag + lineBreak;
            this.active = true;
            this.completed = false;
        }

        public void Complete()
        {
            string strikethroughText = StrikeThrough(this.number.ToString() + ". " + this.originalText);
            this.currentText = completedColorTag + strikethroughText + endColorTag + lineBreak;
            this.active = false;
            this.completed = true;
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
    List<Task> taskList;
    Task activeTask;
    Text text;
    public bool allTasksCompleted = false;

    void Awake()
    {
        text = GameObject.FindGameObjectWithTag("TaskText").GetComponent<Text>();
        taskList = new List<Task>();
        ResetAllTasks();
    }

    public void AddTask(string taskText)
    {
        Task newTask = new Task(taskList.Count + 1, taskText);
        taskList.Add(newTask);
        UpdateTasks();
    }

    public void SetActiveTask(int taskNum)
    {
        foreach (Task t in taskList)
        {
            if (t.active)
                t.Inactive();
        }
        Task task = taskList[taskNum - 1];
        task.Active();
        activeTask = task;
        UpdateTasks();
    }

    public void CompleteTask(int taskNum)
    {
        Task task = taskList[taskNum - 1];
        task.Complete();

        if (!CheckAllTasksCompleted())
        {
            if (task.number < taskList.Count)
                SetActiveTask(taskList[task.number].number);
        }

        UpdateTasks();
    }

    private bool CheckAllTasksCompleted()
    {
        foreach (Task t in taskList)
            if (!t.completed)
                return false;

        Debug.Log("All the day's tasks have been completed!");
        allTasksCompleted = true;
        return true;
        // More to come here later
    }

    public void ResetAllTasks()
    {
        foreach (Task task in taskList)
            task.Reset();
        UpdateTasks();
    }

    void UpdateTasks()
    {
        UpdateTaskUI();
        // Other updates will go in here
    }

    void UpdateTaskUI()
    {
        string tasks = "";
        for (int i = 0; i < taskList.Count; i++)
            tasks += taskList[i].currentText;
        text.text = tasks;
    }
}
