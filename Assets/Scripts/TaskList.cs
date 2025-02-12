using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;

public class TaskList : MonoBehaviour
{
    public Task task;

    public void SetTask(Task taskList)
    {
        task = taskList;
        GetComponent<TMP_Text>().text = task.taskDescription;
    }

    public void SetTaskCompleted()
    {

        GetComponent<TMP_Text>().text = "<s>" + task.taskDescription + "</s>";


    }
}
