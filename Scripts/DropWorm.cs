using UnityEngine;

public class DropWorm : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectRange = 3f;
    [SerializeField] private Vector2 detectOffset = Vector2.zero; // offset from worm center
    [SerializeField] private float coneAngle = 60f;               // full cone angle in degrees
    [SerializeField] private LayerMask playerLayer;

    [Header("Drop & Bounce")]
    [SerializeField] private float bounceForce = 12f;
    [SerializeField] private int bouncesBeforeReset = 2;
    [SerializeField] private float resetDelay = 2f;

    [Header("Caterpillar Bounce")]
    [SerializeField] private float bounceSpeed = 6f;
    [SerializeField] private float squishAmount = 0.2f;
    [SerializeField] private Transform bodyVisual;

    [Header("Damage")]
    [SerializeField] private int damage = 1;

    private Rigidbody2D rb;
    private Vector3 startPos;
    private Vector3 originalScale;
    private float bounceTimer;
    private int bounceCount;
    private bool dropped;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        startPos = transform.position;
        if (bodyVisual == null) bodyVisual = transform;
        originalScale = bodyVisual.localScale;
    }

    void Update()
    {
        CaterpillarBounce();

        if (!dropped && IsPlayerInCone())
            Drop();
    }

    bool IsPlayerInCone()
    {
        // Origin of detection, offset from worm position
        Vector2 origin = (Vector2)transform.position + detectOffset;

        Collider2D hit = Physics2D.OverlapCircle(origin, detectRange, playerLayer);
        if (hit == null) return false;

        // Check if the player is within the downward cone angle
        Vector2 dirToPlayer = (hit.transform.position - (Vector3)origin).normalized;
        float angle = Vector2.Angle(Vector2.down, dirToPlayer);

        return angle <= coneAngle * 0.5f;
    }

    void Drop()
    {
        dropped = true;
        rb.gravityScale = 3f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!dropped) return;

        for (int i = 0; i < col.contactCount; i++)
        {
            if (col.GetContact(i).normal.y > 0.5f)
            {
                bounceCount++;
                if (bounceCount >= bouncesBeforeReset)
                {
                    Invoke(nameof(Reset), resetDelay);
                    return;
                }
                rb.velocity = new Vector2(rb.velocity.x, bounceForce);
                return;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) => DamagePlayer(other);
    void OnTriggerStay2D(Collider2D other) => DamagePlayer(other);

    void DamagePlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage, transform.position);
    }

    void Reset()
    {
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        transform.position = startPos;
        bounceCount = 0;
        dropped = false;
    }

    void CaterpillarBounce()
    {
        bounceTimer += Time.deltaTime * bounceSpeed;
        float bounce = Mathf.Sin(bounceTimer);
        float scaleX = originalScale.x * (1f + squishAmount * -bounce);
        float scaleY = originalScale.y * (1f + squishAmount * bounce);
        bodyVisual.localScale = new Vector3(scaleX, scaleY, originalScale.z);
    }

    void OnDrawGizmosSelected()
    {
        Vector2 origin = (Vector2)transform.position + detectOffset;

        // Draw the detection circle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, detectRange);

        // Draw the cone edges
        Gizmos.color = Color.red;
        float halfAngle = coneAngle * 0.5f;
        Vector3 leftEdge = Quaternion.Euler(0, 0, halfAngle) * Vector2.down * detectRange;
        Vector3 rightEdge = Quaternion.Euler(0, 0, -halfAngle) * Vector2.down * detectRange;
        Gizmos.DrawLine(origin, (Vector2)origin + (Vector2)leftEdge);
        Gizmos.DrawLine(origin, (Vector2)origin + (Vector2)rightEdge);

        // Draw center direction
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + Vector2.down * detectRange);
    }
}