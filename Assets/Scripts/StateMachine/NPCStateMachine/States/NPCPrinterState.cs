using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPrinterState : NPCActiveState
{
    public NPCPrinterState(NPC _npc, NPCStateMachine _stateMachine, string _animBoolName) : base(_npc, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        npc.CURRENT_STATE = NPC.STATES.PRINTER;
        npc.actionFlag = true;
    }

    public override void Update()
    {
        base.Update();
        if (!npc.actionFlag)
        {
            npc.PickRandomWaypoint();
            stateMachine.ChangeState(npc.moveState);
        }

        // fill up a progress bar over time with a 1 in 10 say chance to move on to do something else. when progress bar is full, reset start again and add 20 points to final score.
    }

    public override void Exit()
    {
        base.Exit();
    }
}

