using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{

    
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.CURRENT_STATE = Player.STATES.MOVE;
        
    }

    public override void Update()
    {
        base.Update();

        if (xInput == 0 && yInput == 0)
            stateMachine.ChangeState(player.idleState);
        
        if (player.IsSprinting() && yInput > 0)
        {
            player.SetVelocity(xInput * (player.moveSpeed * 1.5f), yInput * (player.moveSpeed * 1.5f));
        }
        else
        {
            player.SetVelocity(xInput * player.moveSpeed, yInput * player.moveSpeed);
        }
        
        ApplyHeadBob();

    }

    public override void Exit()
    {
        base.Exit();
    }

    private void ApplyHeadBob()
    {
        Vector2 movementInputs = new Vector2(xInput, yInput);
        float inputMagnitude = movementInputs.magnitude;

        if (inputMagnitude > 0)
        {
            player.bobbingTime += Time.deltaTime * player.bobFrequency;

            float verticalOffset = Mathf.Sin(player.bobbingTime) * player.bobAmplitute;
            float horizontalOffset = Mathf.Cos(player.bobbingTime) * player.bobHorizontalAmplitude;

            player.FPCam.transform.localPosition = player.cameraStartPosition + new Vector3(horizontalOffset, verticalOffset, 0f);
        }
        else
        {
            player.bobbingTime = 0f;
            player.FPCam.transform.localPosition = Vector3.Lerp(player.FPCam.transform.localPosition, player.cameraStartPosition, Time.deltaTime * player.bobSmoothSpeed);
        }
    }
}

