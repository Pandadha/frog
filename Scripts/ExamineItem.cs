using UnityEngine;
using System.Collections;

public class ExamineItem : MonoBehaviour
{
    [TextArea(2, 5)]
    public string examineText = "An empty bottle. Still smells sweet.";

    [Header("Bubble")]
    public GameObject bubbleRoot;        // the world-space bubble child object
    public Vector2 offset = new Vector2(0f, 1.2f);

    [Header("Squash & Pop")]
    public float popDuration = 0.25f;
    public AnimationCurve popCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private TMPro.TMP_Text label;
    private Coroutine animRoutine;

    void Awake()
    {
        if (bubbleRoot != null)
        {
            bubbleRoot.transform.localPosition = offset;
            label = bubbleRoot.GetComponentInChildren<TMPro.TMP_Text>();
            if (label != null) label.text = examineText;
            bubbleRoot.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (bubbleRoot == null) return;

        if (animRoutine != null) StopCoroutine(animRoutine);
        bubbleRoot.SetActive(true);
        animRoutine = StartCoroutine(PopIn());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (bubbleRoot == null) return;
        if (!bubbleRoot.activeSelf) return;
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(PopOut());
    }

    IEnumerator PopIn()
    {
        float t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            float eval = popCurve.Evaluate(t / popDuration);

            // squash then settle: wide → tall → normal
            float scaleX = Mathf.LerpUnclamped(1.4f, 1f, eval);
            float scaleY = Mathf.LerpUnclamped(0.6f, 1f, eval);
            bubbleRoot.transform.localScale = new Vector3(scaleX, scaleY, 1f);

            yield return null;
        }
        bubbleRoot.transform.localScale = Vector3.one;
    }

    IEnumerator PopOut()
    {
        float t = 0f;
        Vector3 startScale = bubbleRoot.transform.localScale;
        while (t < popDuration * 0.6f)   // slightly faster out
        {
            t += Time.deltaTime;
            float eval = t / (popDuration * 0.6f);
            bubbleRoot.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, eval);
            yield return null;
        }
        bubbleRoot.SetActive(false);
        bubbleRoot.transform.localScale = Vector3.one;  // reset for next time
    }
}