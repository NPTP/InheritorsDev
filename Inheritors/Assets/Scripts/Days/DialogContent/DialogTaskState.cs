using System;
using System.Collections.Generic;
using UnityEngine;

// Active, Completed, Other, Start

public class DialogTaskState : MonoBehaviour
{
    DialogContent dialogContent;

    void Awake()
    {
        dialogContent = GetComponent<DialogContent>();
    }

    public void SetDialogs(ref Task activeTask,
                            ref Dictionary<TaskType, Task> taskList,
                            ref Dictionary<Character, Dialog> dialogs,
                            ref Dictionary<Character, DialogTrigger> dialogTriggers)
    {
        foreach (Character character in Enum.GetValues(typeof(Character)))
        {
            TaskType type = GetTaskTypeFromCharacter(character);

            // I'm doing this task now.
            if (taskList[type].status == TaskStatus.Active)
                SetDialogTo(ref dialogs, character, "_Active");

            // I've finished this task.
            else if (taskList[type].status == TaskStatus.Completed)
                SetDialogTo(ref dialogs, character, "_Completed");

            // I'm in the middle of someone else's task.
            else if (activeTask.type != TaskType.Null && activeTask.type != type)
                SetDialogTo(ref dialogs, character, "_Other");

            // I haven't done this task yet, and it's ready to be started.
            else if (activeTask.type == TaskType.Null && taskList[type].status == TaskStatus.Waiting)
                SetDialogTo(ref dialogs, character, "_Start");

            // For all other cases (e.g. having no task today at all but being talkable)...
            else
                SetDialogTo(ref dialogs, character, "_Default");
        }
    }

    void SetDialogTo(ref Dictionary<Character, Dialog> dialogs, Character character, string suffix)
    {
        string key = character.ToString() + suffix;
        if (dialogContent.ContainsKey(key)) dialogs[character] = dialogContent.Get(key);
    }

    TaskType GetTaskTypeFromCharacter(Character character)
    {
        switch (character)
        {
            case Character.Mother:
                return TaskType.Mother;

            case Character.Father:
                return TaskType.Father;

            case Character.Sister:
                return TaskType.Sister;

            case Character.Grandfather:
                return TaskType.Grandfather;

            case Character.Grandmother:
                return TaskType.Grandmother;

            default:
                return TaskType.Null;
        }
    }
}
