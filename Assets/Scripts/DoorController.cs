using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform doorHinge; // The point the door rotates around
    [SerializeField] private float openAngle = 90f; // How far the door opens
    [SerializeField] private float openSpeed = 2f; // Speed of door opening
    [SerializeField] private GameObject textUI; // UI text to display interaction prompt

    private bool isOpening = false;
    private bool isDoorOpen = false;
    private Quaternion targetRotation;
    private Quaternion initialRotation;

    void Start()
    {
        // Cache the initial rotation of the door hinge
        initialRotation = doorHinge.rotation;
    }

    void Update()
    {
        if (isOpening)
        {
            doorHinge.rotation = Quaternion.Slerp(doorHinge.rotation, targetRotation, Time.deltaTime * openSpeed);

            // Check if the door has nearly reached the target rotation
            if (Quaternion.Angle(doorHinge.rotation, targetRotation) < 0.1f)
            {
                isOpening = false; // Stop the opening or closing action
                doorHinge.rotation = targetRotation; // Snap to the exact target rotation
            }
        }
    }

    public void EnableOrDisableText(bool enable)
    {
        if (textUI != null)
        {
            textUI.SetActive(enable);
        }
    }

    public void Interact()
    {
        // Only allow interaction if the door is not currently moving
        if (isOpening)
        {
            
            return;
        }

        
        if (isDoorOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor(Camera.main.transform.position);
        }
    }

    private void OpenDoor(Vector3 playerPosition)
    {
        Vector3 directionToPlayer = playerPosition - doorHinge.position;
        Vector3 hingeForward = doorHinge.forward;

        // Determine if the player is in front or behind the door
        float dotProduct = Vector3.Dot(directionToPlayer, hingeForward);

        if (dotProduct > 0)
        {
            // Player is in front of the door; open away from them
            targetRotation = Quaternion.Euler(doorHinge.eulerAngles.x, doorHinge.eulerAngles.y + openAngle, doorHinge.eulerAngles.z);
        }
        else
        {
            // Player is behind the door; open away from them
            targetRotation = Quaternion.Euler(doorHinge.eulerAngles.x, doorHinge.eulerAngles.y - openAngle, doorHinge.eulerAngles.z);
        }

        isOpening = true;
        isDoorOpen = true;
    }

    private void CloseDoor()
    {
        targetRotation = initialRotation;
        isOpening = true;
        isDoorOpen = false;
    }

    public bool IsGrabbableItem()
    {
        return false;
    }
}
