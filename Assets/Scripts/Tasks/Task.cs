using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum TaskStatus
{
    Pending,
    InProgress,
    Completed
}

public enum TaskType
{
    TurnSomethingOn,
    GiveItem,
    ReceiveItem
}

[System.Serializable]
public class Task
{
    public string taskName;
    public string description;
    public int xpReward;
    public TaskStatus status;
    public TaskType taskType;
    public string[] dependencies; 
    public string requiredItem;




    public Task(string taskName, string description, int xpReward, TaskType taskType, string[] dependencies, string requiredItem)
    {
        this.taskName = taskName;
        this.description = description;
        this.xpReward = xpReward;
        this.taskType = taskType;
        this.status = TaskStatus.Pending;
        this.dependencies = dependencies;
        this.requiredItem = requiredItem;
    }

    public void CompleteTask(ref int xp)
    {
        if (status == TaskStatus.InProgress)
        {
            status = TaskStatus.Completed;
            xp += xpReward; // Add XP reward
        }
    }

    // Check if task is ready to be completed (based on dependencies)
    public bool CanCompleteDependencies(List<string> completedTasks)
    {
        foreach (var dep in dependencies)
        {
            
            if (!completedTasks.Contains(dep))
            {
                return false; 
            }
        }
        return true; 
    }
}
