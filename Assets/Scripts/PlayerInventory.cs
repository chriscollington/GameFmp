using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    private HashSet<string> items = new HashSet<string>();

    void Awake()
    {
        instance = this;
    }

    public void AddItem(string itemID)
    {
        items.Add(itemID);
        Debug.Log("Picked up: " + itemID);
    }

    public bool HasItem(string itemID)
    {
        return items.Contains(itemID);
    }
}