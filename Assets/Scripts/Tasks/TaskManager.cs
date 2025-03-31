using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    public int totalScore;

    public int minTimeToGiveTasks;
    public int maxTimeToGiveTasks;

    public List<Task> allTasks = new List<Task>();
    public List<Task> currentTasks = new List<Task>();
    public List<Task> completedTasks = new List<Task>();
    public GameObject[] NPCS;
   
    public List<Task> assignedTasks = new List<Task>();

    public List<GameObject> giveAbleItems = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

 
    public void GetRandomTask()
    {

    }

    private void Start()
    {

        //pickRandomTaskAllNPCS();
        LoadTasksFromJson("tasks.json");
        StartCoroutine(AssignRandomTaskAtIntervals());

    }
     

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) // Example trigger
        {
            Debug.Log("Total Points: " + GetTotalPoints());
        }
    }

    private IEnumerator AssignRandomTaskAtIntervals()
    {
        while (true) // Infinite loop to keep assigning tasks
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minTimeToGiveTasks, maxTimeToGiveTasks)); // Wait for a random interval

            AssignRandomTaskToNPC(); // Call function to assign a task
        }
    }
    public void LoadTasksFromJson(string jsonFileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (File.Exists(filePath))
        {
            Debug.Log("Data Found at " + filePath);
            string jsonData = File.ReadAllText(filePath);

            TaskListJson taskListJson = JsonUtility.FromJson<TaskListJson>(jsonData);

            if (taskListJson == null || taskListJson.tasks == null)
            {
                Debug.LogError("Invalid or empty JSON data.");
                return;
            }

            if (allTasks == null)
            {
                allTasks = new List<Task>();  // Initialize only if it's null
            }

            foreach (TaskJson taskJson in taskListJson.tasks)
            {
                if (Enum.TryParse(taskJson.taskType, out TaskType parsedTaskType))
                {
                    Task newTask = new Task(
                        taskJson.taskName,
                        taskJson.description,
                        taskJson.xpReward,
                        parsedTaskType,
                        taskJson.dependencies,
                        taskJson.requiredItem
                    );

                    allTasks.Add(newTask);  // Append instead of overwriting
                }
                else
                {
                    Debug.LogError($"Invalid taskType: {taskJson.taskType}");
                }
            }
        }
        else
        {
            Debug.LogError("Data not Found at " + filePath);
        }
    }


    // Assign random task to an NPC
    public void AssignRandomTaskToNPC()
    {
        List<string> names = new List<string>();
        if (completedTasks.Count > 0)
        {
            foreach (Task task in completedTasks)
            {
                names.Add(task.taskName);
            }
        }
        
        List<Task> availableTasks = allTasks.FindAll(t => !assignedTasks.Contains(t) && t.CanCompleteDependencies(names));

        if (availableTasks.Count == 0) return;

        Task randomTask = availableTasks[UnityEngine.Random.Range(0, availableTasks.Count)];
        NPC randomNPC = NPCS[UnityEngine.Random.Range(0, NPCS.Length)].GetComponent<NPC>();
        if (randomNPC != null)
        {
            randomNPC.currentTask = randomTask;
        }
        assignedTasks.Add(randomTask);
        
    }


    public void CheckTaskCompletion(Task task, bool skipCheck)
    {
        bool completed = false;

        if (skipCheck)
        {
            completed = true;
            task.status = TaskStatus.Completed;
        }

        switch (task.taskType)
        {
            case TaskType.GiveItem:
                if (Inventory.Instance.ContainsItemByString(task.requiredItem))
                {
                    Inventory.Instance.RemoveItemByString(task.requiredItem);
                    task.status = TaskStatus.Completed;
                    completed = true;
                    RemoveCurrentTask(task);
                }
                    
                break;
            case TaskType.TurnSomethingOn:
                GameObject targetObject = GameObject.Find(task.requiredItem);
                if (targetObject != null)
                {
                    LightSwitch lightSwitch = targetObject.GetComponent<LightSwitch>();


                    if (lightSwitch != null)
                    {
                        if (lightSwitch.active)
                        {
                            completed = true;
                            task.status = TaskStatus.Completed;
                            RemoveCurrentTask(task);
                        }
                    }
                }
                break;


                
        }

        if (completed) 
        {
            task.CompleteTask(ref totalScore);
            completedTasks.Add(task);
        }
    }


    public void RemoveCurrentTask(Task task)
    {
        currentTasks.Remove(task);
        
    }

    public void RemoveAssignedTask(Task task)
    {
        assignedTasks.Remove(task);
    }
    

    public int GetTotalPoints()
    {
        //return completedTasks.Sum(task => task.points);
        return totalScore;
    }

    public GameObject ReturnItemToGive(string name)
    {
        foreach(GameObject item in giveAbleItems)
        {
            if (item.name == name)
            {
                return item;
            }
        }

        return null;
    }

    [System.Serializable]
    public class TaskList
    {
        public Task[] tasks;
    }

    [System.Serializable]
    public class TaskJson
    {
        public string taskName;
        public string description;
        public int xpReward;
        public string taskType; // Store as string from JSON
        public string[] dependencies;
        public string requiredItem;
    }

    [System.Serializable]
    public class TaskListJson
    {
        public TaskJson[] tasks;
    }
}
