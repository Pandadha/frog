using System.Collections;
using UnityEngine;
public class FalseWall : MonoBehaviour
{
    public float hiddenAlpha = 0.25f;
    public float fadeSpeed = 3f;

    private SpriteRenderer sr;
    private float originalAlpha;
    private Coroutine currentFade;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalAlpha = sr.color.a;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            StartFade(hiddenAlpha);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            StartFade(originalAlpha);
    }

    void StartFade(float target)
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeTo(target));
    }

    IEnumerator FadeTo(float target)
    {
        while (!Mathf.Approximately(sr.color.a, target))
        {
            Color c = sr.color;
            c.a = Mathf.MoveTowards(c.a, target, fadeSpeed * Time.deltaTime);
            sr.color = c;
            yield return null;
        }
    }
}