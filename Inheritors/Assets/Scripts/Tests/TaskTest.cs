﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskTest : MonoBehaviour
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

    void Start()
    {
        text = GameObject.FindGameObjectWithTag("TaskText").GetComponent<Text>();
        // TaskManager should get tasks from the DayManager which pulls
        // from some ScriptableObject full of them or something.
        // Then TaskManager updates the tasks for all interested parties:
        // UI text, in-game shiet, etc.

        // For now, we'll just manually put some tasks in.

        Task tutorialTask1 = new Task(1, "Get wood for the fire");
        Task tutorialTask2 = new Task(2, "Put wood on the fire");
        Task tutorialTask3 = new Task(3, "Listen to mama's story");

        taskList = new List<Task>();
        taskList.Add(tutorialTask1);
        taskList.Add(tutorialTask2);
        taskList.Add(tutorialTask3);

        activeTask = taskList[0];

        ResetAllTasks();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetActiveTask(taskList[0]);
            UpdateTasks();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetActiveTask(taskList[1]);
            UpdateTasks();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetActiveTask(taskList[2]);
            UpdateTasks();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CompleteTask(activeTask);
        }
    }

    void SetActiveTask(Task task)
    {
        foreach (Task t in taskList)
        {
            if (t.active)
                t.Inactive();
        }
        task.Active();
        activeTask = task;
    }

    void CompleteTask(Task task)
    {
        task.Complete();

        if (!CheckAllTasksCompleted())
        {
            if (task.number < taskList.Count)
                SetActiveTask(taskList[task.number]);
        }

        UpdateTasks();
    }

    bool CheckAllTasksCompleted()
    {
        foreach (Task t in taskList)
            if (!t.completed)
                return false;

        Debug.Log("All the day's tasks have been completed!");
        return true;
        // More to come here later
    }

    void ResetAllTasks()
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
