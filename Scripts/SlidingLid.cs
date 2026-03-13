using System.Collections;
using UnityEngine;

public class SlidingLid : MonoBehaviour
{
    public Vector2 slideDirection = Vector2.right;
    public float slideDistance = 2f;
    public float slideSpeed = 3f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + (Vector3)(slideDirection.normalized * slideDistance);
    }

    public void Open()
    {
        StopAllCoroutines();
        StartCoroutine(SlideTo(openPosition));
    }

    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(SlideTo(closedPosition));
    }

    IEnumerator SlideTo(Vector3 target)
    {
        Vector3 start = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
    }
}