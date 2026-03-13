using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("References")]
    public GameObject inventoryPanel;
    public Transform gridParent;
    public GameObject slotPrefab;

    [Header("Toggle Key")]
    public KeyCode toggleKey = KeyCode.Tab;

    private bool isOpen;
    private InventorySlot selectedSlot;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        inventoryPanel.SetActive(false);
        InventorySystem.Instance.OnInventoryChanged += Refresh;
    }

    void OnDestroy()
    {
        if (InventorySystem.Instance != null)
            InventorySystem.Instance.OnInventoryChanged -= Refresh;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) Toggle();
    }

    void Toggle()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
        if (isOpen) Refresh();
        else
        {
            selectedSlot = null;
            ItemDetailPanel.Instance.Clear();
        }
    }

    void Refresh()
    {
        if (!isOpen) return;

        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        selectedSlot = null;
        InventorySlot firstSlot = null;

        foreach (var kvp in InventorySystem.Instance.GetAllItems())
        {
            GameObject go = Instantiate(slotPrefab, gridParent);
            InventorySlot slot = go.GetComponent<InventorySlot>();
            slot.Setup(kvp.Key, kvp.Value);

            if (firstSlot == null) firstSlot = slot;
        }

        // auto select first slot
        if (firstSlot != null) SelectSlot(firstSlot);
        else ItemDetailPanel.Instance.Clear();
    }

    public void SelectSlot(InventorySlot slot)
    {
        if (selectedSlot != null) selectedSlot.SetSelected(false);
        selectedSlot = slot;
        selectedSlot.SetSelected(true);
        ItemDetailPanel.Instance.Show(slot.GetData());
    }
}