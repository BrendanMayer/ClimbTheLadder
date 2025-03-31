using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMoveState : NPCActiveState
{
    public NPCMoveState(NPC _npc, NPCStateMachine _stateMachine, string _animBoolName) : base(_npc, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        npc.CURRENT_STATE = NPC.STATES.MOVE;
    }

    public override void Update()
    {
        base.Update();
        Debug.Log(Vector3.Distance(npc.transform.position, npc.currentWaypoint.transform.position));
        if (Vector3.Distance(npc.transform.position, npc.currentWaypoint.transform.position) < 1)
        {
            npc.CheckForAction();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

