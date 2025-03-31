using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFlags : MonoBehaviour
{
    NPC npc;
    void Start()
    {
        npc = GetComponentInParent<NPC>();
    }


    public void SetActionFlagFalse()
    {
        if (npc != null)
        {
            npc.actionFlag = false;
        }
    }
}

  
