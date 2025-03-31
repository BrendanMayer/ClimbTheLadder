using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    // Singleton instance
    public static Inventory Instance { get; private set; }

    public GameObject playerHandSlot;
    public GameObject currentHeldItem;
    public bool itemHeld = false;
    // Maximum size of the inventory
    private const int MaxItems = 3;

    // List to hold GameObjects (items)
    private List<GameObject> items;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Ensure only one instance of Inventory exists
        if (Instance == null)
        {
            Instance = this;
            items = new List<GameObject>();
        }
        else
        {
            Destroy(gameObject); // Destroy if another instance exists
        }
    }

    // Add an item to the inventory
    public bool AddItem(GameObject item)
    {
        if (items.Count < MaxItems)
        {
            items.Add(item);
            
            if (item.GetComponent<ItemContext>() != null)
            {
                ItemContext data = item.GetComponent<ItemContext>();
                UIManager.instance.SetInventorySprite(data.sprite, items.Count - 1);

            }
            AudioManager.Instance.PlaySoundOnMainSource(AudioManager.Instance.pickup);
            return true;
        }
        else
        {
           
            return false;
        }
    }

    // Remove an item from the inventory
    public bool RemoveItem(GameObject item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            if (item.GetComponent<ItemContext>() != null)
            {
                
                UIManager.instance.RemoveInventorySprite(items.Count);
                for (int i = 0; i < items.Count; i++)
                {
                    ItemContext data = items[i].GetComponent<ItemContext>();
                    UIManager.instance.SetInventorySprite(data.sprite, i);
                }

            }
            return true;
        }
        else
        {
           
            return false;
        }
    }

    public void RemoveItemByString(string item)
    {
        foreach (GameObject item2 in items)
        {
            if (item2.name.Equals(item))
            {
                items.Remove(item2);
                return;
            }
        }
    }

    // Check if the inventory contains a specific item
    public bool ContainsItem(GameObject item)
    {
        return items.Contains(item);
    }

    public bool ContainsItemByString(string item)
    {
        foreach (GameObject item2 in items)
        {
            if (item2.name.Equals(item)) return true;
        }
        return false;
    }

    // Get all items currently in the inventory
    public List<GameObject> GetItems()
    {
        return new List<GameObject>(items);
    }

    // Check if the inventory is full
    public bool IsFull()
    {
        return items.Count >= MaxItems;
    }

    // Optional: Clear the inventory
    public void ClearInventory()
    {
        items.Clear();
        Debug.Log("Inventory cleared.");
    }

    private void PutAllItemsAway()
    {
        foreach (GameObject item in items)
        {
            item.SetActive(false);
            
        }
        
    }

    private void PutItemAway(GameObject item)
    {
        foreach (GameObject item2 in items)
            if (item2 == item)
            {
                item2.SetActive(false);
            }
    }

    public void TakeOutItem(int index)
    {
        if (playerHandSlot == null)
        {
            Debug.LogWarning("Player hand slot not assigned.");
            return;
        }

        if (index < 0 || index >= items.Count)
        {
            Debug.LogWarning("Index out of bounds.");
            return;
        }

        if (items[index] != null)
        {
            
            GameObject item = items[index];
            if (itemHeld && currentHeldItem == item)
            {
                PutItemAway(item);
                currentHeldItem = null;
                itemHeld = false;
                UIManager.instance.NoSelectedSlot();
                return;
            }
            else
            {
                PutAllItemsAway();

                currentHeldItem = item;
                Rigidbody rb = item.GetComponent<Rigidbody>();
                item.GetComponent<Grabbable>().isBeingHeld = true;
                rb.useGravity = false;
                rb.isKinematic = true;
                item.GetComponent<Collider>().enabled = false;
                item.transform.parent = playerHandSlot.transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
                item.SetActive(true);
                itemHeld = true;
                UIManager.instance.SelectInventorySlot(index);
            }
        }
        
    }


    public void DropItem()
    {
        if (playerHandSlot == null)
        {
            Debug.LogWarning("Player hand slot not assigned.");
            return;
        }


        UIManager.instance.NoSelectedSlot();


        // Restore parent and Rigidbody properties
        currentHeldItem.transform.parent = null;
        RemoveItem(currentHeldItem);
        Rigidbody rb = currentHeldItem.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        currentHeldItem.GetComponent<Collider>().enabled = true;
        itemHeld = false;
        currentHeldItem.GetComponent<Grabbable>().isBeingHeld = false;
        currentHeldItem = null;

    }
}
