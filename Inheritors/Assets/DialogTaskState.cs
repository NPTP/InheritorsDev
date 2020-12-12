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
            {
                dialogs[character] = dialogContent.Get(character.ToString() + "_Active");
            }

            // I've finished this task.
            else if (taskList[type].status == TaskStatus.Completed)
            {
                dialogs[character] = dialogContent.Get(character.ToString() + "_Completed");
            }

            // I'm in the middle of someone else's task.
            else if (activeTask.type != TaskType.Null && activeTask.type != type)
            {
                dialogs[character] = dialogContent.Get(character.ToString() + "_Other");
            }

            // I haven't done this task yet, and it's available.
            else if (activeTask.type == TaskType.Null && taskList[type].status == TaskStatus.Waiting)
            {
                dialogs[character] = dialogContent.Get(character.ToString() + "_Start");
            }
        }
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
