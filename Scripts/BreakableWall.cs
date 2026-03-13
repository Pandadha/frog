using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class BreakableWall : MonoBehaviour
{
    public enum BreakSide
    {
        Any,
        FromLeft,
        FromRight,
        FromTop,
        FromBottom
    }

    [Header("Mode")]
    public BreakSide breakSide = BreakSide.Any;

    [Range(-1f, 1f)]
    [Tooltip("Higher = stricter. 0.4 forgiving, 0.7 strict.")]
    public float sideDotThreshold = 0.5f;

    [Header("Min Speed to Break")]
    [Tooltip("Player must be moving at least this fast on impact.")]
    public float minBreakSpeed = 8f;

    [Header("Visuals")]
    public GameObject intactVisual;
    public GameObject brokenVisual;

    [Header("Events")]
    public UnityEvent onBroken;

    private bool broken;
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        if (intactVisual != null) intactVisual.SetActive(true);
        if (brokenVisual != null) brokenVisual.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (broken) return;
        if (!collision.collider.CompareTag("Player")) return;

        Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        // Use collision normal instead of velocity
        Vector2 collisionNormal = collision.GetContact(0).normal;
        Debug.Log($"velocity: {playerRb.velocity}, normal: {collisionNormal}, speed: {collision.relativeVelocity.magnitude}");

        if (collision.relativeVelocity.magnitude < minBreakSpeed) return;

        TryBreak(collisionNormal);
    }

    void TryBreak(Vector2 travelDir)
    {
        if (breakSide == BreakSide.Any)
        {
            Break();
            return;
        }

        Vector2 required = GetRequiredDirection(breakSide);
        float dot = Vector2.Dot(travelDir, required);
        Debug.Log($"travelDir: {travelDir}, required: {required}, dot: {dot}, threshold: {sideDotThreshold}");

        if (dot < sideDotThreshold) return;
        Break();
    }

    Vector2 GetRequiredDirection(BreakSide side)
    {
        switch (side)
        {
            case BreakSide.FromLeft: return Vector2.right;
            case BreakSide.FromRight: return Vector2.left;
            case BreakSide.FromTop: return Vector2.down;
            case BreakSide.FromBottom: return Vector2.up;
            default: return Vector2.right;
        }
    }

    void Break()
    {
        if (broken) return;
        broken = true;
        gameObject.SetActive(false); // or use Destroy(gameObject);
        onBroken?.Invoke();
    }
}