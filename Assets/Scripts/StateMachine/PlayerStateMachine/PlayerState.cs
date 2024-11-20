using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerState : MonoBehaviour
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    
    private string animBoolName;

    #region InputValues
    protected float xInput;
    protected float yInput;
    protected float mouseX;
    protected float mouseY;
    #endregion

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
        
    }

    public virtual void Enter()
    {
        
    }

    public virtual void Update()
    {
        Vector2 movementDir = player.GetMovementDirNormalized();
        xInput = movementDir.x;
        yInput = movementDir.y;

        if (player.DropItem() && player.currentHeldItem != null)
        {
            player.currentHeldItem.GetComponent<Grabbable>().Drop();
            player.currentHeldItem.GetComponent<Grabbable>().EnableCollider();
            player.currentHeldItem = null;
        }
        
    }

    public virtual void Exit()
    {

    }
    
}
