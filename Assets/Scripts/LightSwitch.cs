using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractable
{
    public bool active;
    [SerializeField] private List<Light> lights; // Lights controlled by this switch
    [SerializeField] private GameObject interactText; // UI text for interaction prompt
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        UpdateLights(); 
    }

    void UpdateLights()
    {
        if (lights.Count > 0)
        {
            foreach (Light light in lights)
            {
                light.enabled = active; 
            }
        }
    }

    public void EnableOrDisableText(bool enable)
    {
        if (interactText != null)
        {
            interactText.SetActive(enable);
        }
    }

    public void Interact()
    {
        ToggleActive();
        
    }

    public void ToggleActive()
    {
        active = !active; 
        animator.SetBool("On", active); 
        UpdateLights(); 
    }

    public bool IsGrabbableItem()
    {
        return false;
    }
}
