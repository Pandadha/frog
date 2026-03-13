using UnityEngine;
using UnityEngine.Events;

public class Switch : Interactable
{
    [Header("Visuals")]
    public GameObject offVisual;
    public GameObject onVisual;

    [Header("What happens")]
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;

    [HideInInspector] public bool activated;

    protected override void OnInteract()
    {
        if (activated) Deactivate();
        else Activate();
    }

    void Activate()
    {
        activated = true;
        SetVisual(true);
        onActivate?.Invoke();
    }

    void Deactivate()
    {
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