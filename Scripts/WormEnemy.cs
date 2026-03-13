using UnityEngine;

public class WormEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float startDirection = 1f; // 1 = right/up, -1 = left/down

    [Header("Wall Mode")]
    [SerializeField] private bool wallMode = false;

    [Header("Caterpillar Bounce")]
    [SerializeField] private float bounceSpeed = 6f;
    [SerializeField] private float bounceHeight = 0.08f;
    [SerializeField] private float squishAmount = 0.2f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private Transform bodyVisual;

    private Vector2 startPos;
    private float direction;
    private float traveledDistance = 0f;
    private float bounceTimer = 0f;
    private Vector3 originalScale;

    void Start()
    {
        startPos = transform.position;
        direction = Mathf.Sign(startDirection); // clamp to exactly 1 or -1
        if (bodyVisual == null) bodyVisual = transform;
        originalScale = bodyVisual.localScale;
    }

    void Update()
    {
        Move();
        CaterpillarBounce();
    }

    void Move()
    {
        float move = moveSpeed * direction * Time.deltaTime;
        traveledDistance += Mathf.Abs(move);

        if (wallMode)
            transform.position += new Vector3(0f, move);
        else
            transform.position += new Vector3(move, 0f);

        if (traveledDistance >= patrolDistance)
        {
            direction *= -1f;
            traveledDistance = 0f;
        }
    }

    void CaterpillarBounce()
    {
        bounceTimer += Time.deltaTime * bounceSpeed;
        float bounce = Mathf.Sin(bounceTimer);

        // Use Mathf.Abs on originalScale.x so flipping via direction never double-negates
        float scaleX = Mathf.Abs(originalScale.x) * direction * (1f + squishAmount * -bounce);
        float scaleY = originalScale.y * (1f + squishAmount * bounce);
        bodyVisual.localScale = new Vector3(scaleX, scaleY, originalScale.z);

        Vector3 pos = bodyVisual.localPosition;
        pos.y = bounce * bounceHeight;
        bodyVisual.localPosition = pos;
    }

    void OnTriggerEnter2D(Collider2D other) => DamagePlayer(other);
    void OnTriggerStay2D(Collider2D other) => DamagePlayer(other);

    void DamagePlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage, transform.position);
    }
}