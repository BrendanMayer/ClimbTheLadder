using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class NPCState : MonoBehaviour
{
    protected NPCStateMachine stateMachine;
    protected NPC npc;
    
    private string animBoolName;

    public NPCState(NPC _npc, NPCStateMachine _stateMachine, string _animBoolName)
    {
        this.npc = _npc;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
        
    }

    public virtual void Enter()
    {
        npc.animator.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {
        
    }

    public virtual void Exit()
    {
        npc.animator.SetBool(animBoolName, false);
    }
    
}
