using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.CURRENT_STATE = Player.STATES.JUMP;
        player.rb.velocity = new Vector3(player.rb.velocity.x, player.jumpHeight, player.rb.velocity.z);
    }

    public override void Update()
    {
        base.Update();
        if (player.rb.velocity.y < 0)
        {
            stateMachine.ChangeState(player.airState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

