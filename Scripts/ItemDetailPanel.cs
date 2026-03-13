using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailPanel : MonoBehaviour
{
    public static ItemDetailPanel Instance { get; private set; }

    [Header("References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public Image iconImage;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        Clear();
    }

    public void Show(ItemData item)
    {
        nameText.text = item.itemName;
        descText.text = item.description;
        iconImage.sprite = item.icon;
    }

    public void Clear()
    {
        nameText.text = "";
        descText.text = "";
        iconImage.sprite = null;
    }
}