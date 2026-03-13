using UnityEngine;

public class ItemPickup : Interactable
{
    public ItemData itemData;
    public int quantity = 1;

    protected override void OnInteract()
    {
        if (itemData == null) return;
        InventorySystem.Instance.AddItem(itemData, quantity);
        PickupNotification.Instance.Show(itemData, quantity);
        Destroy(gameObject);
    }
}