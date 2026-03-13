using UnityEngine;

public class AbilityItemPickup : Interactable
{
    public enum AbilityType { WallStick, Squeeze }
    public AbilityType ability;
    public ItemData itemData; // just for the notification

    protected override void OnInteract()
    {
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        if (pm == null) return;

        switch (ability)
        {
            case AbilityType.WallStick: pm.wallStickUnlocked = true; break;
            case AbilityType.Squeeze: pm.squeezeUnlocked = true; break;
        }

        if (itemData != null)
            PickupNotification.Instance.ShowBigNotification(itemData);
            InventorySystem.Instance.AddItem(itemData, 1);
        Destroy(gameObject);
    }
}