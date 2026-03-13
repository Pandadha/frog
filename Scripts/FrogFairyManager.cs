using System.Collections.Generic;
using UnityEngine;


public class FrogFairyManager : MonoBehaviour
{
    public static FrogFairyManager Instance { get; private set; }

    [Header("All 8 fairies in the scene")]
    public FrogFairy[] allFairies; // drag all 8 in here

    private HashSet<int> freedIDs = new HashSet<int>();

    // reward thresholds
    private bool reward3Given = false;
    private bool reward8Given = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // hide all home visuals on start
        foreach (var f in allFairies)
            f.InitHomeVisual();
    }

    public void MarkFreed(int id)
    {
        if (freedIDs.Contains(id)) return;
        freedIDs.Add(id);

        Debug.Log($"[FairyManager] Freed fairy #{id}. Total freed: {freedIDs.Count}");

        CheckRewards();
    }

    public bool IsFreed(int id) => freedIDs.Contains(id);

    public int FreedCount => freedIDs.Count;

    void CheckRewards()
    {
        if (!reward3Given && freedIDs.Count >= 3)
        {
            reward3Given = true;
            Debug.Log("[FairyManager] REWARD 1 unlocked! (3 fairies freed) - replace with real reward");
        }

        if (!reward8Given && freedIDs.Count >= 8)
        {
            reward8Given = true;
            Debug.Log("[FairyManager] REWARD 2 unlocked! (all 8 fairies freed) - replace with real reward");
        }
    }

    // Call this when player enters the fairy home room
    // Hook this into DoorTransition or a room trigger
    public void OnPlayerEnterFairyRoom()
    {
        Debug.Log($"[FairyManager] Player entered fairy room. Showing {freedIDs.Count} freed fairies.");
        foreach (var f in allFairies)
        {
            if (IsFreed(f.fairyID))
                f.ShowInHomeRoom();
        }
    }
}