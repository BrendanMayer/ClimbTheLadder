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
        UIManager.instance.SetNameText(player.currentTalkingToNPC.GetComponent<ChatGPTManager>().traits.name);

    }

    public override void Update()
    {
        base.Update();
        if (player.CloseNPCChat())
        {
            player.talkingToNPC = false;
            
            UIManager.instance.StopTypingAndClear();
            UIManager.instance.ToggleChatWindow(false);
            player.currentTalkingToNPC.stateMachine.ChangeState(player.currentTalkingToNPC.idleState);
            player.currentTalkingToNPC = null;
            stateMachine.ChangeState(player.idleState);
        }

        if (player.VoiceInput())
        {
            
            if (!player.recording)
            {
                player.recording = true;
                UIManager.instance.SetMicrophoneIndicator(true);
                player.GetComponent<Whisper>().StartRecording();
            }
            else if (player.recording)
            {
                player.recording = false;
                UIManager.instance.SetMicrophoneIndicator(false);
                player.GetComponent<Whisper>().EndRecording();
            }
        }
    }

    public override void Exit()
    {
     
        
        base.Exit();
    }
}

