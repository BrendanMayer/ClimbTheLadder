using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComputerState : PlayerState
{
    public PlayerComputerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.UnlockMouseLockCamera();
       
    }

    public override void Update()
    {
        base.Update();
        if (player.computerSitPosition != null)
        {
            // Smoothly interpolate the position of the player
            player.transform.position = Vector3.Lerp(
                player.transform.position,
                player.computerSitPosition.position,
                Time.deltaTime * player.cameraLerpSpeed
            );

            // Smoothly interpolate the rotation of the player
            player.transform.rotation = Quaternion.Lerp(
                player.transform.rotation,
                player.computerSitPosition.rotation,
                Time.deltaTime * player.cameraLerpSpeed
            );

            // Smoothly adjust the camera to look at player.computerCamPosition
            if (player.computerCamPosition != null)
            {
                Camera.main.transform.rotation = Quaternion.Slerp(
                    Camera.main.transform.rotation,
                    Quaternion.LookRotation(player.computerCamPosition.position - Camera.main.transform.position),
                    Time.deltaTime * player.cameraLerpSpeed
                );
            }

            // Check if the player is close enough to the target
            if (Vector3.Distance(player.transform.position, player.computerSitPosition.position) < player.threshold &&
                Quaternion.Angle(player.transform.rotation, player.computerSitPosition.rotation) < player.threshold)
            {
                // Snap to the target to avoid endless small adjustments
                player.transform.position = player.computerSitPosition.position;
                player.transform.rotation = player.computerSitPosition.rotation;

                // Snap the camera's rotation to look at the target
                if (player.computerCamPosition != null)
                {
                    Camera.main.transform.rotation = Quaternion.LookRotation(player.computerCamPosition.position - Camera.main.transform.position);
                }
            }

        }
    


        if (player.IsInteracting())
        {
            player.LockMouse();
            stateMachine.ChangeState(player.idleState);
        }

        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("powerButton"))
                {
                    hit.collider.gameObject.GetComponentInParent<Computer>().OnPowerButtonClicked();

                }
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

