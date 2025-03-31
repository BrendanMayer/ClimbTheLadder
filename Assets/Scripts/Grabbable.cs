using UnityEngine;

public class Grabbable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform grabOffset;
    private Rigidbody rb; // Reference to the object's Rigidbody
    public bool isBeingHeld = false;

    private Transform originalParent; // Store original parent for releasing the object
    public Transform playerHandSlot; // Reference to the player's hand slot
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerHandSlot = GameObject.Find("HandSlot").transform;
    }

    public void SetPlayerHandSlot(Transform handSlot)
    {
        playerHandSlot = handSlot;
    }

    public void EnableOrDisableText(bool enable)
    {
        // Display or hide interaction text
        
    }

    public void Interact()
    {
        
        if (!isBeingHeld ) 
        {
            Grab();
        }
    }

    

    private void Grab()
    {
        if (!Inventory.Instance.IsFull())
        {
            Inventory.Instance.AddItem(this.gameObject);
            this.gameObject.SetActive(false);
        }
        
    }

    

    public bool IsGrabbableItem()
    {
        return true;
    }

    public void EnableCollider()
    {
        GetComponent<Collider>().enabled = true;
    }

    public void DisableCollider()
    {
        GetComponent<Collider>().enabled = false;
    }
}
