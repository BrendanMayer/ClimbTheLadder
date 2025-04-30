

using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public NPCStateMachine stateMachine { get; private set; }
    #region Components
    public Animator animator { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public ChatGPTManager manager { get; private set; }
    #endregion

    public float idleTimerMax = 5f;
    public float idleTimer;
    public GameObject[] waypoints;
    public GameObject[] actionWaypoints;
    public GameObject currentWaypoint;

    public Task currentTask = null;
    public Task dependencyTask;
    

    bool hasGivenPlayerTask;

    public bool hasTaskBeenCompletedAtLeastOnce;
   
    public bool hasBeenSpokenTo = false;
    public bool currentlyTalking = false;
    public bool actionFlag = false;

    public bool dontMove = false;

    public Transform itemSpawner;
    public GameObject taskIcon;
    public Slider progressBar;
    public float fillDuration = 5f; // Fill time in seconds
    public CoworkerPCData currentPC;
    public bool isRunning;
    #region States



    public enum STATES
    {
        IDLE,
        MOVE,
        TALKING,
        INTRO,
        PRINTER,
        COMPUTER,
    }

    public STATES CURRENT_STATE;

    public NPCIdleState idleState { get; private set; }
    public NPCMoveState moveState { get; private set; }
    public NPCDialogueIntroState introState { get; private set; }
    public NPCDialogueState dialogueState { get; private set; }
    public NPCPrinterState printerState {  get; private set; }
    public NPCComputerState computerState { get; private set; }
    public float rotationSpeed = 1f;

    #endregion


    private void Awake()
    {

        stateMachine = new NPCStateMachine();

        idleState = new NPCIdleState(this, stateMachine, "Idle");
        moveState = new NPCMoveState(this, stateMachine, "Move");
        introState = new NPCDialogueIntroState(this, stateMachine, "Intro");
        dialogueState = new NPCDialogueState(this, stateMachine, "Dialogue");
        printerState = new NPCPrinterState(this, stateMachine, "Printer");
        computerState = new NPCComputerState(this, stateMachine, "Computer");

    }


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        manager = GetComponent<ChatGPTManager>();
        stateMachine.Initialize(idleState);
        idleTimer = idleTimerMax;
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        actionWaypoints = GameObject.FindGameObjectsWithTag("ActionWaypoint");

     

    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.currentState.Update();
        taskIcon.SetActive(currentTask.taskName != "" && currentTask.status != TaskStatus.Completed);
    }

    public void StartProgressBar()
    {
        StartCoroutine(FillProgress());
    }

    IEnumerator FillProgress()
    {
        while (isRunning)
        {
            float timer = 0f;
            progressBar.value = 0f;

            while (timer < fillDuration)
            {
                timer += Time.deltaTime;
                progressBar.value = Mathf.Clamp01(timer / fillDuration);
                yield return null;
            }

            // Full bar reached

            GameManager.Instance.productivityScore += 5;

            // 1 in 10 chance to stop
            if (Random.Range(1, 11) == 1) // Random 1–10
            {

                isRunning = false;
                actionFlag = false;
                yield break;
            }
        }

    }

    public bool PickRandomWaypoint()
    {
        if (!dontMove)
        {
            if (waypoints == null || waypoints.Length < 1)
            {
                Debug.Log("No Waypoints Found");
                return false;
            }
            int randomIndex = Random.Range(0, waypoints.Length);
            Debug.Log("Waypoint: " + randomIndex);
            agent.SetDestination(waypoints[randomIndex].transform.position);
            currentWaypoint = waypoints[randomIndex];
            return true;
        }

        return false;
        

    }

    public bool PickRandomActionWaypoint()
    {
        if (!dontMove)
        {
            if (actionWaypoints == null || actionWaypoints.Length < 1)
            {
                Debug.Log("No Waypoints Found");
                return false;
            }
            int randomIndex = Random.Range(0, actionWaypoints.Length);

            agent.SetDestination(actionWaypoints[randomIndex].transform.position);
            currentWaypoint = actionWaypoints[randomIndex];
            return true;
        }
        return false;

    }

    public void CheckForAction()
    {
        ActionType actionType;
        if (actionType = currentWaypoint.GetComponent<ActionType>())
        {
            switch (actionType.type)
            {
                case ActionType.ActionTypes.Printer:
                    stateMachine.ChangeState(printerState);
                    break;
                case ActionType.ActionTypes.Computer:
                    stateMachine.ChangeState(computerState);
                    break;
                case ActionType.ActionTypes.None:
                    stateMachine.ChangeState(idleState);
                    break;
                default:
                    break;
            }
                

        }
        
    }

    public string TaskMessage()
    {
        string entryMessage = "";
        if (currentTask.taskName != "" && currentTask.taskType != TaskType.ReceiveItem)
        {
            Debug.Log("Task available: " + currentTask.description);
            entryMessage += "You are speaking to the player. Start with a greeting. Do not mention tasks until the player asks about them. " +
                "If the player asks for a task, respond with the task description: " + currentTask.description + ". " +
                "Only when the player explicitly asks for a task, append `--givetask` at the end of your response. ";

            if (currentTask.requiredItem != "" && currentTask.taskType != TaskType.ReceiveItem)
            {
                GameObject item = GameObject.Find(currentTask.requiredItem);
                if (item != null)
                {
                    entryMessage += "This task requires an item, but **do not mention the item unless the player asks about it**. " +
                    "If asked, give vague hints about the item: `" + GameObject.Find(currentTask.requiredItem).GetComponent<ItemContext>().itemLocation + "`, `" +
                    GameObject.Find(currentTask.requiredItem).GetComponent<ItemContext>().dependency + "`. " +
                    "Never reveal this information unless prompted. ";
                }
                
            }
        }
        else if (currentTask.taskName != "" && currentTask.taskType == TaskType.ReceiveItem)
        {
            Debug.Log("fired correct message");
            entryMessage += "You are speaking to the player, say hello! and have an item to give: `" + currentTask.requiredItem + "`. " +
                "**Do not mention the item until the player asks about it.** " +
                "If the player asks for the item, you must respond and add `--giveitem` at the end of your response. do not use --givetask";
        }
        else if (currentTask.taskName != "")
        {
            entryMessage += "You are speaking to the player, say hello! and have a main task to give: `" + currentTask.description + "`. " +
                "**However, this task has a prerequisite task that must be completed first.** " +
                "The prerequisite task is: `" + dependencyTask.description + "`. " +
                "**Rules:** " +
                "1. Do not give the main task until the player completes the prerequisite task. " +
                "2. If the player asks for the main task, tell them they need to complete the prerequisite task first. " +
                "3. Only when you actually give the main task, add `--givetask` at the end of the response. ";
        }
        else
        {
            Debug.Log("No tasks available for the player.");
            entryMessage += "You are speaking to the player, but you have no tasks to give them. Respond naturally.";
        }

        return entryMessage;

        
    }

    public void IntroMessgage()
    {
        // move to intro state
        string text = "";
        if (currentPC != null && currentPC.hasError)
        {
            text = "Your PC ran into a problem! tell the player to have a look if there is a virus or a network area through their computer terminal!";
        }
        else if (!hasBeenSpokenTo && GameManager.Instance.powerOn)
        {
            text = TaskMessage();
            hasBeenSpokenTo = true;
        }
        else if (!GameManager.Instance.powerOn)
        {
            text = "The lights are off, tell the player that the breaker is downstairs and they need to turn it back on so you can get back to work! You wont offer any other dialogue except asking to turn the lights back on";
        }
        else if (hasGivenPlayerTask && currentTask.status == TaskStatus.Completed && hasTaskBeenCompletedAtLeastOnce)
        {
            text = "If the player asks for help to complete a task, you only help others if there's something in it for you.\r\nThe player has previously completed a task for you, so you're now open to the idea of helping them—but not without being convinced.\r\nWhen the player asks you to do something, your first reaction should be: \"What's in it for me?\"\r\nRespond based on your specific likes and dislikes (as previously defined).\r\nIf the player's persuasion appeals to your preferences, you agree and respond helpfully, ending your message with --doTask to signal task acceptance.\r\nIf not, politely refuse or ask for more convincing.";
        }
        else if (hasGivenPlayerTask && currentTask.status != TaskStatus.Completed)
        {
            text = "You gave me your task already, tell me to complete it";
        }
        else if (hasGivenPlayerTask && currentTask.status == TaskStatus.Completed)
        {
            text = "I have completed your task!";

        }
        
        else
        {
            text = "I started speaking to you again, Say something!";
        }
        
        
        

        manager.AskChatGPT(text);
    }

    internal void GiveTask()
    {
        TaskManager.Instance.currentTasks.Add(currentTask);
        UIManager.instance.CreateTaskListItem(currentTask);
        hasGivenPlayerTask = true;
    }

    internal void GiveItem()
    {
        GameObject item = TaskManager.Instance.ReturnItemToGive(currentTask.requiredItem);
        TaskManager.Instance.CheckTaskCompletion(currentTask, true);
        if (Inventory.Instance.IsFull())
        {
            // if full spit it out else add to inventory
            Instantiate(item, itemSpawner);
        }
        else
        {
            Inventory.Instance.AddItem(item);
        }
        
        

    }
}
