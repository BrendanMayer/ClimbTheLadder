using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCComputerState : NPCState
{
    public NPCComputerState(NPC _npc, NPCStateMachine _stateMachine, string _animBoolName) : base(_npc, _stateMachine, _animBoolName)
    {

    }


    CoworkerPCData pc;


    public override void Enter()
    {
        base.Enter();
        npc.CURRENT_STATE = NPC.STATES.COMPUTER;
        npc.actionFlag = true;
        npc.isRunning = true;
        npc.progressBar.gameObject.SetActive(true);
        npc.StartProgressBar();

        // MOVE NPC TO CHAIR
        npc.agent.isStopped = true; // Stop any pathfinding
        npc.agent.enabled = false;
        npc.transform.position = npc.currentWaypoint.transform.position;

        // Manually rotate to match chair orientation
        npc.transform.rotation = npc.currentWaypoint.transform.rotation;

        pc = npc.currentWaypoint.GetComponent<CoworkerPCData>();
    }

    public override void Update()
    {
        base.Update();
        if (!npc.actionFlag)
        {
            npc.PickRandomWaypoint();
            stateMachine.ChangeState(npc.moveState);
        }

        if (pc.hasError)
        {
            npc.taskIcon.GetComponent<TMP_Text>().color = Color.red;
            
        }
        else
        {
            npc.taskIcon.GetComponent<TMP_Text>().color = Color.white;
            
        }
        
    }

    public override void Exit()
    {
        // move out of chair
        npc.agent.enabled = true;
        npc.agent.isStopped = false; // Stop any pathfinding
        npc.progressBar.gameObject.SetActive(false);
        base.Exit();
    }

   
}

