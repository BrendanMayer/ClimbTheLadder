using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCIdleState : NPCState
{
    public NPCIdleState(NPC _npc, NPCStateMachine _stateMachine, string _animBoolName) : base(_npc, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        npc.CURRENT_STATE = NPC.STATES.IDLE;
    }

    public override void Update()
    {
        base.Update();
        
        // Check For Waypoint To Move To

        if (npc.idleTimer >= 0)
        {
            npc.idleTimer -= Time.deltaTime;
        }
        else
        {
            bool foundWaypoint = npc.PickRandomWaypoint();
            Debug.Log("foundWaypoint call equals: " + foundWaypoint);
            npc.idleTimer = npc.idleTimerMax; // Reset timer
            if (foundWaypoint)
            {
                stateMachine.ChangeState(npc.moveState);
            }
            
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

