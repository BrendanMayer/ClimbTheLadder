using Samples.Whisper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour, IInteractable
{
    public Material powerOff;
    public Material powerOn;
    public bool power;
    public GameObject powerButton;
    Player player;
    public GameObject UI;
   
    public GameObject settingsWindow;
    public GameObject menu;

    public void EnableOrDisableText(bool enable)
    {
        
    }

    public void Interact()
    {
        player.stateMachine.ChangeState(player.computerState);
    }

    public bool IsGrabbableItem()
    {
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();

        CloseAllWindows();
        UI.SetActive(false);
    }

    private void CloseAllWindows()
    {
        
        settingsWindow.SetActive(false);
        menu.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        // Check if the mouse is over a GameObject and if it's the correct one
        
    }

    public void ToggleRecording(bool toggle)
    {
        player.GetComponent<Whisper>().menuOpen = toggle;
    }

    public void OnPowerButtonClicked()
    {
        
        power = !power;
        UI.SetActive(power);
        if (power)
        {
            powerButton.GetComponent<Renderer>().material = powerOn;
            menu.SetActive(true);
        }
        else
        {
            powerButton.GetComponent<Renderer>().material = powerOff;
            CloseAllWindows();
        }

        
    }
}
