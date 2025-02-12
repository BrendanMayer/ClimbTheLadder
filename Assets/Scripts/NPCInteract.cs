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
        TaskManager.Instance.CheckForTaskCompletion();
        player.currentTalkingToNPC = GetComponent<ChatGPTManager>();
        player.stateMachine.ChangeState(player.interactState);
        player.GetComponent<Whisper>().currentTalkingToNPC = GetComponent<ChatGPTManager>();
       

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
