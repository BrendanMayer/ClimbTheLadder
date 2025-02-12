using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMoveState : NPCState
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
    }

    public override void Exit()
    {
        base.Exit();
    }
}

