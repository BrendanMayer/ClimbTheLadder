using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueState : NPCActiveState
{
    public NPCDialogueState(NPC _npc, NPCStateMachine _stateMachine, string _animBoolName) : base(_npc, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        npc.CURRENT_STATE = NPC.STATES.TALKING;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
        npc.currentlyTalking = false;
    }
}

