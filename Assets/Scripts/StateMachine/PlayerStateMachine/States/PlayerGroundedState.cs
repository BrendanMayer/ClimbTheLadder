using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    

    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if (player.IsJumping())
        {
            stateMachine.ChangeState(player.jumpState);
        }

        
        player.PerformRaycast();
    }

    public override void Exit()
    {
        base.Exit();
        
    }

    
    
}


