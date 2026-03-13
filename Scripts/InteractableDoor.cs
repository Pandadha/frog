using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractableDoor : Interactable
{
    [Header("Where this door goes")]
    public RoomTransition targetRoom;
    public Transform targetSpawnPoint;

    [Header("Fade")]
    public Image flashImage;
    public float fadeSpeed = 5f;

    [Header("Lock")]
    public bool requiresKey = false;
    public ItemData requiredItem;        // drag your key ItemData asset here
    public bool consumeKeyOnUse = true;
    public GameObject lockedPromptObject;

    private static bool isTransitioning;
    private CameraFollow cameraFollow;

    void Start()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (flashImage != null) flashImage.color = new Color(0, 0, 0, 0);
        if (lockedPromptObject != null) lockedPromptObject.SetActive(false);
    }

    protected override void OnInteract()
    {
        if (isTransitioning) return;

        if (requiresKey)
        {
            if (!InventorySystem.Instance.HasItem(requiredItem))
            {
                ShowLockedFeedback();
                return;
            }

            if (consumeKeyOnUse)
                InventorySystem.Instance.RemoveItem(requiredItem);
        }

        StartCoroutine(DoDoorTransition());
    }

    IEnumerator DoDoorTransition()
    {
        isTransitioning = true;
        cameraFollow.enabled = false;

        yield return Fade(0f, 1f);

        if (targetRoom != null)
        {
            cameraFollow.SetRoomBounds(targetRoom.GetBounds());
            if (targetRoom.roomBackgroundColor != Color.clear)
                Camera.main.backgroundColor = targetRoom.roomBackgroundColor;
        }

        if (targetSpawnPoint != null)
        {
            // Find player by tag since we don't have a collider reference here
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) player.transform.position = targetSpawnPoint.position;
        }

        cameraFollow.SnapNow();
        yield return new WaitForSeconds(0.05f);

        cameraFollow.enabled = true;
        yield return Fade(1f, 0f);

        isTransitioning = false;
    }

    IEnumerator Fade(float from, float to)
    {
        if (flashImage == null) yield break;
        float elapsed = 0f;
        float duration = 1f / Mathf.Max(0.01f, fadeSpeed);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            flashImage.color = new Color(0, 0, 0, Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        flashImage.color = new Color(0, 0, 0, to);
    }

    void ShowLockedFeedback()
    {
        if (lockedPromptObject != null)
            StartCoroutine(FlashLockedPrompt());
    }

    IEnumerator FlashLockedPrompt()
    {
        lockedPromptObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        lockedPromptObject.SetActive(false);
    }
}