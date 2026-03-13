using System.Collections;
using UnityEngine;

// Attach this to each trapped fairy frog GameObject (8 total)
// The same GameObject gets placed in both the trap location AND the home room
// - In trap: it's the breakable cage visual, freed by BreakableWall.onBroken
// - In home room: it starts hidden, appears when player enters the room

public class FrogFairy : MonoBehaviour
{
    [Header("Identity")]
    public int fairyID; // 0-7, must be unique per fairy

    [Header("Trap")]
    public GameObject trapVisual;       // the cage/trap sprite shown before freeing
    public Vector2 flyDirection = new Vector2(-1f, 1f); // pretend direction home
    public float flySpeed = 4f;
    public float flyDuration = 1.2f;

    [Header("Home Room")]
    public GameObject homeVisual;       // the fairy sprite shown in their home room
                                        // place this in the home room at desired position

    // called by BreakableWall.onBroken Unity Event
    public void OnFreed()
    {
        FrogFairyManager.Instance.MarkFreed(fairyID);

        if (trapVisual != null)
            StartCoroutine(FlyAway());
    }

    IEnumerator FlyAway()
    {
        // detach fairy sprite from cage so it doesn't disappear with it
        transform.SetParent(null);

        // now hide the cage
        trapVisual.SetActive(false);

        // fly the fairy (now we're moving the root FrogFairy_01 object)
        float elapsed = 0f;
        Vector2 dir = flyDirection.normalized;

        while (elapsed < flyDuration)
        {
            elapsed += Time.deltaTime;
            transform.position += (Vector3)(dir * flySpeed * Time.deltaTime);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    // Called by FrogFairyManager when player enters the home room
    public void ShowInHomeRoom()
    {
        if (homeVisual != null)
            homeVisual.SetActive(true);
    }

    // Call this on scene start - fairies that haven't been freed stay hidden in home room
    public void InitHomeVisual()
    {
        if (homeVisual != null)
            homeVisual.SetActive(FrogFairyManager.Instance.IsFreed(fairyID));
    }
}