using AOT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCActiveState : NPCState
{
    public NPCActiveState(NPC _npc, NPCStateMachine _stateMachine, string _animBoolName) : base(_npc, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        GameObject player = GameObject.Find("Player");
        if (player != null && npc.currentlyTalking)
        {
            Vector3 direction = player.transform.position - npc.transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            npc.transform.rotation = Quaternion.Lerp(npc.transform.rotation, targetRotation, Time.deltaTime * npc.rotationSpeed);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

