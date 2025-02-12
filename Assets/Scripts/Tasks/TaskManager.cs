using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    public Task[] possibleTasks;
    public List<Task> currentTasks = new List<Task>();
    public GameObject[] NPCS;
    private Player player;
    HashSet<int> assignedTasks = new HashSet<int>(); // Tracks assigned task indices

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    private void pickRandomTask()
    {
        if (possibleTasks.Length == 0 || NPCS.Length == 0)
            return; // Safety check: No tasks or no NPCs

       
        

        foreach (GameObject npc in NPCS)
        {
            ChatGPTManager manager = npc.GetComponent<ChatGPTManager>();

            if (manager == null)
                continue; // Skip NPCs without a ChatGPTManager

            // Get a unique task index that hasn’t been assigned yet
            int roll;
            int maxAttempts = possibleTasks.Length;
            int attempt = 0;

            do
            {
                roll = Random.Range(0, possibleTasks.Length);
                attempt++;
            }
            while (assignedTasks.Contains(roll) && attempt < maxAttempts);

            // If all tasks are assigned, stop assigning (prevents infinite loops)
            if (assignedTasks.Count >= possibleTasks.Length)
                break;

            assignedTasks.Add(roll); // Mark this task as assigned
            manager.currentTask = possibleTasks[roll];

            // Load personality if a new task was assigned
            if (manager.currentTask != null)
            {
                manager.LoadPersonality();
            }
        }

    }

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        pickRandomTask();
    }

    public void CheckForTaskCompletion()
    {
        if (currentTasks.Count() > 0)
        {
            foreach (Task task  in currentTasks)
            {
                task.IsTaskCompleted(player);
            }
            
        }
    }
}
