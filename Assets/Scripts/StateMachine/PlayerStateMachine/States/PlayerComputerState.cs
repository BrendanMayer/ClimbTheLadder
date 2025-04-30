using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerComputerState : PlayerState
{

    private Vector3 originalLocation;
    private Quaternion originalRotation;

    public PlayerComputerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        originalLocation = player.transform.position;
        originalRotation = player.transform.rotation;
        player.CURRENT_STATE = Player.STATES.COMPUTER;
        player.UnlockMouseLockCamera();
        player.playerCanvas.SetActive(false);
        //player.playerCollider.enabled = false;
        player.rb.useGravity = false;
        player.pcChairCollider.enabled = false;
        player.pcDeskCollider.enabled = false;
       
    }

    public override void Update()
    {
        base.Update();


        LookAtComputer();


        if (player.IsInteracting() && !player.terminalOpen)
        {
            player.LockMouse();
            stateMachine.ChangeState(player.idleState);
        }

        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 2, Color.red);    

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("powerButton"))
                {
                    hit.collider.gameObject.GetComponentInParent<Computer>().OnPowerButtonClicked();
                    AudioManager.Instance.PlaySoundOnMainSource(AudioManager.Instance.clickComputerButton);

                }
            }
        }
    }

    private void LookAtComputer()
    {
        if (player.computerSitPosition != null)
        {
            // Check if the player is close enough to the target
            bool closeToTarget = Vector3.Distance(player.transform.position, player.computerSitPosition.position) < player.threshold &&
                                 Quaternion.Angle(player.transform.rotation, player.computerSitPosition.rotation) < player.threshold;

            if (closeToTarget)
            {
                // Snap to the target to avoid endless small adjustments
                player.transform.position = player.computerSitPosition.position;
                player.transform.rotation = player.computerSitPosition.rotation;

                if (player.computerCamPosition != null)
                {
                    Camera.main.transform.rotation = Quaternion.LookRotation(
                        player.computerCamPosition.position - Camera.main.transform.position
                    );
                }

                return; // Stop further interpolation this frame
            }

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
        }
    }

    public override void Exit()
    {
        player.playerCanvas.SetActive(true);
        //player.playerCollider.enabled = true;
        player.rb.useGravity = true;
        player.transform.position = originalLocation;
        player.transform.rotation = originalRotation;
        player.pcChairCollider.enabled = true;
        player.pcDeskCollider.enabled = true;
        
        base.Exit();
    }
}

