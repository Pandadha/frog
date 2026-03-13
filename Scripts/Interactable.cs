using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    [Header("Prompt")]
    public GameObject promptObject; // drag a child TMP object here
    public Vector2 promptOffset = new Vector2(0f, 1.5f);

    private bool playerInRange;
    private static Interactable current;

    protected virtual void OnInteract() { }

    void Awake()
    {
        if (promptObject != null)
        {
            promptObject.transform.localPosition = promptOffset;
            promptObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        current = this;
        playerInRange = true;
        if (promptObject != null) promptObject.SetActive(true);
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (current == this)
        {
            current = null;
            if (promptObject != null) promptObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
            OnInteract();
    }
}