using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject offVisual;
    public GameObject onVisual;

    [Header("One shot or toggle")]
    public bool oneShot = false;

    [Header("What happens")]
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

    [HideInInspector] public bool activated;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneShot && activated) return;
        activated = true;
        SetVisual(true);
        onActivate?.Invoke();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneShot) return;
        activated = false;
        SetVisual(false);
        onDeactivate?.Invoke();
    }

    void SetVisual(bool on)
    {
        if (offVisual) offVisual.SetActive(!on);
        if (onVisual) onVisual.SetActive(on);
    }
}