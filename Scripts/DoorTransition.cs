using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DoorTransition : MonoBehaviour
{
    [Header("Where this door goes")]
    public RoomTransition targetRoom;     // bounds to activate
    public Transform targetSpawnPoint;    // where player appears in target room

    [Header("Fade")]
    public Image flashImage;
    public float fadeSpeed = 5f;

    private static bool isTransitioning;
    private CameraFollow cameraFollow;

    void Start()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (flashImage != null) flashImage.color = new Color(0, 0, 0, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // IMPORTANT: Only the player ROOT triggers this
        Debug.Log($"Door hit by: {other.name} tag={other.tag} rb={other.attachedRigidbody?.name}");
        if (!other.CompareTag("Player")) return;

        Debug.Log(" PlayerBody triggered door");
        if (isTransitioning) return;

        StartCoroutine(DoDoorTransition(other.transform.root));
    }

    IEnumerator DoDoorTransition(Transform playerRoot)
    {
        isTransitioning = true;

        // (Optional) disable camera follow while transitioning
        cameraFollow.enabled = false;

        // Fade out
        yield return Fade(0f, 1f);

        // Activate new room bounds
        if (targetRoom != null)
        {
            cameraFollow.SetRoomBounds(targetRoom.GetBounds());
            // Change background color
            if (targetRoom.roomBackgroundColor != Color.clear)
                Camera.main.backgroundColor = targetRoom.roomBackgroundColor;
        }
        // Teleport player a little inside the new room
        if (targetSpawnPoint != null)
            playerRoot.position = targetSpawnPoint.position;

        // Snap camera immediately (prevents seeing old room)
        cameraFollow.SnapNow();

        // Small pause (optional)
        yield return new WaitForSeconds(0.05f);

        cameraFollow.enabled = true;

        // Fade in
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
            float a = Mathf.Lerp(from, to, elapsed / duration);
            flashImage.color = new Color(0, 0, 0, a);
            yield return null;
        }

        flashImage.color = new Color(0, 0, 0, to);
    }
}
