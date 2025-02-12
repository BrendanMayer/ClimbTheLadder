using UnityEngine;

public enum TaskType { Dialogue, ItemDelivery, TurnSomethingOn }

[System.Serializable]
public class Task
{
    public string taskDescription;
    public TaskType taskType;
    public string targetNPC;
    public string requiredItem;

    public bool IsTaskCompleted(Player player)
    {
        switch (taskType)
        {
            case TaskType.Dialogue:
               // return player.HasSpokenTo(targetNPC);
            case TaskType.ItemDelivery:
                UIManager.instance.CompleteTaskListItem(this, player.HasItem(requiredItem));
                return player.HasItem(requiredItem);
            case TaskType.TurnSomethingOn:
                UIManager.instance.CompleteTaskListItem(this, player.HasTurnedOn(requiredItem));
                return player.HasTurnedOn(requiredItem);
            default:
                return false;
        }
    }
}
