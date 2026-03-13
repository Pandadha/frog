using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupNotification : MonoBehaviour
{
    public static PickupNotification Instance { get; private set; }

    [Header("Small Notification")]
    public Image iconImage;
    public TextMeshProUGUI itemNameText;
    public RectTransform panel;

    [Header("Big Notification")]
    public Image bigIconImage;
    public TextMeshProUGUI bigNameText;
    public TextMeshProUGUI bigDescText;
    public RectTransform bigPanel;

    [Header("Timing")]
    public float holdDuration = 2f;
    public float holdBigDuration = 5f;
    public float popDuration = 0.2f;
    public float holdScaleX = 1.15f;
    public float holdScaleY = 0.85f;

    private Coroutine showRoutine;
    private Coroutine bigShowRoutine;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        panel.gameObject.SetActive(true);
        bigPanel.gameObject.SetActive(true);
        panel.localScale = Vector3.zero;
        bigPanel.localScale = Vector3.zero;
    }

    public void Show(ItemData item, int quantity = 1)
    {
        if (showRoutine != null) StopCoroutine(showRoutine);
        if (bigShowRoutine != null)
        {
            StopCoroutine(bigShowRoutine);
            bigPanel.localScale = Vector3.zero;
        }
        showRoutine = StartCoroutine(DoShow(item, quantity));
    }

    public void ShowBigNotification(ItemData item)
    {
        if (bigShowRoutine != null) StopCoroutine(bigShowRoutine);
        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            panel.localScale = Vector3.zero;
        }
        bigShowRoutine = StartCoroutine(DoShowBig(item));
    }

    IEnumerator DoShow(ItemData item, int quantity = 1)
    {
        iconImage.sprite = item.icon;
        itemNameText.text = item.itemName;

        yield return ScaleTo(panel, Vector3.zero, new Vector3(holdScaleX, holdScaleY, 1f), popDuration);
        yield return ScaleTo(panel, new Vector3(holdScaleX, holdScaleY, 1f), Vector3.one, popDuration * 0.5f);
        yield return new WaitForSeconds(holdDuration);
        yield return ScaleTo(panel, Vector3.one, new Vector3(holdScaleX, holdScaleY, 1f), popDuration * 0.5f);
        yield return ScaleTo(panel, new Vector3(holdScaleX, holdScaleY, 1f), Vector3.zero, popDuration);
    }

    IEnumerator DoShowBig(ItemData item)
    {
        bigIconImage.sprite = item.icon;
        bigNameText.text = item.itemName;
        bigDescText.text = item.description;

        yield return ScaleTo(bigPanel, Vector3.zero, new Vector3(holdScaleX, holdScaleY, 1f), popDuration);
        yield return ScaleTo(bigPanel, new Vector3(holdScaleX, holdScaleY, 1f), Vector3.one, popDuration * 0.5f);
        yield return new WaitForSeconds(holdBigDuration);
        yield return ScaleTo(bigPanel, Vector3.one, new Vector3(holdScaleX, holdScaleY, 1f), popDuration * 0.5f);
        yield return ScaleTo(bigPanel, new Vector3(holdScaleX, holdScaleY, 1f), Vector3.zero, popDuration);
    }

    // now takes a target panel so both methods can share it
    IEnumerator ScaleTo(RectTransform target, Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(from, to, t / duration);
            yield return null;
        }
        target.localScale = to;
    }
}