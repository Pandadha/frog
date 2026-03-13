using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    // item -> quantity
    private Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();

    public event System.Action OnInventoryChanged;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        if (items.ContainsKey(item))
            items[item] += amount;
        else
            items[item] = amount;

        Debug.Log($"Added {amount}x {item.itemName}. Total: {items[item]}");
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        if (!items.ContainsKey(item)) return;

        items[item] -= amount;
        if (items[item] <= 0)
            items.Remove(item);

        OnInventoryChanged?.Invoke();
    }

    public bool HasItem(ItemData item, int amount = 1)
    {
        return items.ContainsKey(item) && items[item] >= amount;
    }

    public Dictionary<ItemData, int> GetAllItems() => items;
}