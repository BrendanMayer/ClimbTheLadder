using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.CURRENT_STATE = Player.STATES.IDLE;
    }

    public override void Update()
    {
        base.Update();

        if (xInput != 0 || yInput != 0)
            stateMachine.ChangeState(player.moveState);

        
            
        
    }

    public override void Exit()
    {
        base.Exit();
    }

}
