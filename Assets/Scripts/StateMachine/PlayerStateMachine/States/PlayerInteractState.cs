using Samples.Whisper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractState : PlayerGroundedState
{
    public PlayerInteractState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        UIManager.instance.ToggleChatWindow(true);
        //UIManager.instance.InitiateDialogue();
        player.currentTalkingToNPC.IntroMessgage();
    }

    public override void Update()
    {
        base.Update();
        if (player.CloseNPCChat())
        {
            player.talkingToNPC = false;
            UIManager.instance.StopTypingAndClear();
            UIManager.instance.ToggleChatWindow(false);

            stateMachine.ChangeState(player.idleState);
        }

        if (player.VoiceInput())
        {
            
            if (!player.recording)
            {
                player.recording = true;
                Debug.Log("Recording!!!");
                player.GetComponent<Whisper>().StartRecording();
            }
            else if (player.recording)
            {
                player.recording = false;
                Debug.Log("Stopped Recording!!!");
                player.GetComponent<Whisper>().EndRecording();
            }
        }
    }

    public override void Exit()
    {
     
        
        base.Exit();
    }
}

