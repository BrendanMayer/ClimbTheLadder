using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    private IInteractable currentInteractable;

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
        PerformRaycast();
    }

    public override void Exit()
    {
        base.Exit();
        
    }

    private void PerformRaycast()
    {
        
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

       
        Ray ray = player.FPCam.ScreenPointToRay(screenCenter);

        
        if (Physics.Raycast(ray, out RaycastHit hit, player.interactDistance, player.interactableLayers))
        {

            IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();

            if (interactable != null)
            {
                
                if (currentInteractable != interactable)
                {
                    
                    if (currentInteractable != null)
                    {
                        currentInteractable.EnableOrDisableText(false);
                    }

                    
                    interactable.EnableOrDisableText(true);
                    currentInteractable = interactable;
                }
            }
            else if (currentInteractable != null)
            {
               
                currentInteractable.EnableOrDisableText(false);
                currentInteractable = null;
            }
        }
        else if (currentInteractable != null)
        {
          
            currentInteractable.EnableOrDisableText(false);
            currentInteractable = null;
        }


        if (currentInteractable != null && player.IsInteracting())
        {
            currentInteractable.Interact();
            if (currentInteractable.IsGrabbableItem())
            {

                player.GetCurrentHeldObject();
                
            }
        }
    }
    
}


