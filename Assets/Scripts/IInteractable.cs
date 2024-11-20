public interface IInteractable
{
    void EnableOrDisableText(bool enable); // Method to show/hide text
    void Interact(); // Method to handle interaction (e.g., toggle light, open door)

    bool IsGrabbableItem();
}