using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCanvas : MonoBehaviour
{
    GameObject player;
    void Start()
    {
        player = GameObject.Find("Player");
    }

    
    void Update()
    {
        transform.LookAt(player.transform.position);
    }
}
