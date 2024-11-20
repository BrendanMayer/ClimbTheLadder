using UnityEngine;

public class Grabbable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform grabOffset; // Optional offset for correct orientation when grabbed
    private Rigidbody rb; // Reference to the object's Rigidbody
    private bool isBeingHeld = false;

    private Transform originalParent; // Store original parent for releasing the object
    public Transform playerHandSlot; // Reference to the player's hand slot
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        Debug.Log("Interacting");
        if (isBeingHeld)
        {
            Drop();
        }
        else
        {
            Grab();
        }
    }

  

    private void Grab()
    {
        if (playerHandSlot == null)
        {
            Debug.LogWarning("Player hand slot not assigned.");
            return;
        }

        isBeingHeld = true;

        // Store the original parent
        originalParent = transform.parent;

        // Disable gravity and make Rigidbody kinematic
        rb.useGravity = false;
        rb.isKinematic = true;

        // Parent the object to the player's hand slot
        transform.parent = playerHandSlot;

        // Align position and rotation
        transform.localPosition = Vector3.zero;
        if (grabOffset != null)
        {
            transform.localRotation = grabOffset.localRotation;
        }
        else
        {
            transform.localRotation = Quaternion.identity;
        }
    }

    public void Drop()
    {
        isBeingHeld = false;

        // Restore parent and Rigidbody properties
        transform.parent = originalParent;
        rb.useGravity = true;
        rb.isKinematic = false;

        // Optionally add a small force or interaction logic here
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
