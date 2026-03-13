using UnityEngine;

// Attach this to the fairy home room's DoorTransition GameObject
// OR as a separate trigger zone at the room entrance
// It calls FrogFairyManager.OnPlayerEnterFairyRoom when player arrives

public class FairyRoomTrigger : MonoBehaviour
{
    // Option A: call this from DoorTransition's existing OnTriggerEnter2D
    // by adding a UnityEvent to DoorTransition and wiring it here
    // 
    // Option B (simplest for prototype): just add this component to the 
    // fairy room entrance collider with IsTrigger = true

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        FrogFairyManager.Instance.OnPlayerEnterFairyRoom();
    }
}