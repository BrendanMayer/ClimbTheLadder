using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueIntroState : NPCActiveState
{
    public NPCDialogueIntroState(NPC _npc, NPCStateMachine _stateMachine, string _animBoolName) : base(_npc, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        npc.CURRENT_STATE = NPC.STATES.INTRO;
        npc.currentlyTalking = true;

        

        
        npc.IntroMessgage();

        // change to dialogueState
        npc.stateMachine.ChangeState(npc.dialogueState);
    }

    public override void Update()
    {
        base.Update();
        
    }

    public override void Exit()
    {
        if (npc.currentTask.status == TaskStatus.Completed)
        {
            TaskManager.Instance.RemoveAssignedTask(npc.currentTask);
            npc.currentTask = null;
        }

        base.Exit();
    }
}

