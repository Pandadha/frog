using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public Image icon;
    public TextMeshProUGUI quantityText;
    public Image selectionBorder; // separate Image child, just an outline sprite

    private ItemData data;

    public void Setup(ItemData itemData, int quantity)
    {
        data = itemData;
        icon.sprite = itemData.icon;
        quantityText.text = quantity > 1 ? quantity.ToString() : "";
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectionBorder) selectionBorder.enabled = selected;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (data == null) return;
        InventoryUI.Instance.SelectSlot(this);
        ItemDetailPanel.Instance.Show(data);
    }

    public ItemData GetData() => data;
}