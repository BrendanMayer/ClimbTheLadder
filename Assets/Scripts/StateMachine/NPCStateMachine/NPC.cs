using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NPC : MonoBehaviour
{
    public NPCStateMachine stateMachine { get; private set; }
    public Animator animator { get; private set; }
    public NavMeshAgent agent { get; private set; }

    public float idleTimerMax = 5f;
    public float idleTimer;
    public GameObject[] waypoints;
    #region States

    public enum STATES
    {
        IDLE,
        MOVE,
        TALKING,
        INTRO
    }

    public STATES CURRENT_STATE;

    public NPCIdleState idleState { get; private set; }
    public NPCMoveState moveState { get; private set; }

    #endregion


    private void Awake()
    {

        stateMachine = new NPCStateMachine();

        idleState = new NPCIdleState(this, stateMachine, "Idle");
        moveState = new NPCMoveState(this, stateMachine, "Move");
    }


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        stateMachine.Initialize(idleState);
        idleTimer = idleTimerMax;
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.currentState.Update();
    }

    public bool PickRandomWaypoint()
    {
        if (waypoints == null || waypoints.Length < 1)
        {
            Debug.Log("No Waypoints Found");
            return false;
        }
        int randomIndex = Random.Range(0, waypoints.Length);
        Debug.Log("Waypoint: " +  randomIndex);
        agent.SetDestination(waypoints[randomIndex].transform.position);

        return true;
        
    }
}
