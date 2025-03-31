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
        if (!npc.actionFlag )
        {
            npc.PickRandomWaypoint();
            stateMachine.ChangeState(npc.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

