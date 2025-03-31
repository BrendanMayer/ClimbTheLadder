using Samples.Whisper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : MonoBehaviour, IInteractable
{
    Player player;

    public void EnableOrDisableText(bool enable)
    {
        
    }

    public void Interact()
    {
        player.talkingToNPC = true;
        
        
        NPC npc = GetComponent<NPC>();

        player.currentTalkingToNPC = npc;
        if (npc.currentTask.taskName != "")
        {
            TaskManager.Instance.CheckTaskCompletion(npc.currentTask, false);
        }
        
        player.stateMachine.ChangeState(player.interactState);
        player.GetComponent<Whisper>().currentTalkingToNPC = GetComponent<ChatGPTManager>();
        
        npc.stateMachine.ChangeState(npc.introState);
       

    }

    public bool IsGrabbableItem()
    {
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
