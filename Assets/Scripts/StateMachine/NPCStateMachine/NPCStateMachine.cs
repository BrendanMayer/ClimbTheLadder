using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStateMachine
{
    public NPCState currentState {  get; private set; }

    public void Initialize(NPCState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(NPCState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
