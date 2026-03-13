using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Walk")]
    public float maxWalkSpeed = 7f;
    public float accel = 60f;
    public float airControl = 0.5f; // 0..1 (less control in air)

    [Header("Wall Stick")]
    public float wallCheckDist = 0.6f;
    public LayerMask wallLayer;
    public float wallSlideSpeed = -1f;

    [Header("Squeeze")]
    public float squeezeScaleX = 0.6f;
    public float squeezeScaleY = 0.5f;
    public Vector2 squeezeColliderSize = new Vector2(0.5f, 0.5f);

    private bool squeezing;
    public float squeezeColliderRadius = 0.3f;

    private float originalColliderRadius;
    private CircleCollider2D col;

    [HideInInspector] public bool wallStickUnlocked = false;
    [HideInInspector] public bool squeezeUnlocked = false;

    [Header("Gate Visual")]
    public SpriteRenderer gateArc;   // half-circle sprite showing the allowed zone
    public Transform gateDot;        // dot that follows mouse direction
    public float gateRadius = 1.2f;  // distance from center to dot

    [Header("Gate Angles (degrees)")]
    public float minAngle = 20f;     // leftmost allowed launch angle
    public float maxAngle = 160f;    // rightmost allowed launch angle

    [Header("Charge Force")]
    public float minForce = 10f;
    public float maxForce = 18f;
    public float chargeTime = 0.7f;

    [Header("Optional Squash")]
    public bool squash = true;
    public float squashX = 1.25f;
    public float squashY = 0.7f;

    [Header("Frog Feel")]
    public float gravityScale = 3.5f;
    public float fastFallMultiplier = 2f;
    public float landingSquashDuration = 0.08f;

    private float landingVelocityY;

    private Rigidbody2D rb;
    private Vector3 originalScale;

    private bool onWall;
    private int wallSide;
    private bool chargingOnWall;

    private bool charging;
    private float charge01;

    // Current angle pointed at by mouse, clamped to gate range
    private float gateAngle;

    private bool grounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        gateAngle = (minAngle + maxAngle) * 0.5f; // start pointing straight up-ish
        rb.gravityScale = gravityScale;

        col = GetComponent<CircleCollider2D>();
        originalColliderRadius = col.radius;
    }

    void Update()
    {
        // ---- Walk ----
        float x = Input.GetAxisRaw("Horizontal");
        float control = grounded ? 1f : airControl;

        float targetVx = x * maxWalkSpeed;
        float newVx = Mathf.MoveTowards(rb.velocity.x, targetVx, accel * control * Time.deltaTime);
        rb.velocity = new Vector2(newVx, rb.velocity.y);

        if (!grounded )
            landingVelocityY = rb.velocity.y;

        // ---- Wall Stick ----
        bool holdLeft = Input.GetKey(KeyCode.A) || Input.GetAxisRaw("Horizontal") < -0.1f;
        bool holdRight = Input.GetKey(KeyCode.D) || Input.GetAxisRaw("Horizontal") > 0.1f;

        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDist, wallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDist, wallLayer);

        onWall = false;
        wallSide = 0;

        if (!grounded && wallStickUnlocked)
        {
            if (hitLeft && holdLeft) { onWall = true; wallSide = -1; }
            if (hitRight && holdRight) { onWall = true; wallSide = 1; }
        }

        if (onWall || (charging && chargingOnWall))
        {
            rb.gravityScale = 0f;
            float slideVy = Mathf.Min(rb.velocity.y, wallSlideSpeed);
            rb.velocity = new Vector2(0f, slideVy);
        }

        // Fast fall
        if (!onWall && !(charging && chargingOnWall))
        {
            rb.gravityScale = rb.velocity.y < 0f
                ? gravityScale * fastFallMultiplier
                : gravityScale;
        }

        // ---- Squeeze ----
        if (squeezeUnlocked && Input.GetKeyDown(KeyCode.S))
        {
            squeezing = true;
            col.radius = squeezeColliderRadius;
            transform.localScale = new Vector3(
                originalScale.x * squeezeScaleX,
                originalScale.y * squeezeScaleY,
                originalScale.z);
        }

        if (squeezing && Input.GetKeyUp(KeyCode.S))
        {
            squeezing = false;
            col.radius = originalColliderRadius;
            if (!charging)
                transform.localScale = originalScale;
        }

        // ---- Begin Charge ----
        if (Input.GetKeyDown(KeyCode.Space) && (grounded || onWall))
        {
            charging = true;
            charge01 = 0f;
            chargingOnWall = onWall;
        }

        // ---- While Charging ----
        if (charging && Input.GetKey(KeyCode.Space))
        {
            charge01 += Time.deltaTime / Mathf.Max(0.0001f, chargeTime);
            charge01 = Mathf.Clamp01(charge01);

            // --- Mouse aim ---
            // Get mouse position in world space, compute angle from player
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            Vector2 toMouse = mouseWorld - transform.position;

            float rawAngle = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;

            // Clamp to the allowed gate range
            gateAngle = Mathf.Clamp(rawAngle, minAngle, maxAngle);

            // Squash while charging
            if (squash && !squeezing)
            {
                float sx = Mathf.Lerp(1f, squashX, charge01);
                float sy = Mathf.Lerp(1f, squashY, charge01);
                transform.localScale = new Vector3(
                    originalScale.x * sx,
                    originalScale.y * sy,
                    originalScale.z);
            }

            // Show gate visuals
            if (gateArc) gateArc.enabled = true;
            if (gateDot) gateDot.gameObject.SetActive(true);

            // Move dot to current aim direction
            Vector2 aimDir = new Vector2(
                Mathf.Cos(gateAngle * Mathf.Deg2Rad),
                Mathf.Sin(gateAngle * Mathf.Deg2Rad));
            gateDot.localPosition = (Vector3)(aimDir.normalized * gateRadius);
        }
        else
        {
            // Hide gate when not charging
            if (gateArc) gateArc.enabled = false;
            if (gateDot) gateDot.gameObject.SetActive(false);
        }

        // ---- Release Space = launch ----
        if (charging && Input.GetKeyUp(KeyCode.Space))
        {
            charging = false;
            if (squash) transform.localScale = originalScale;

            if (grounded || chargingOnWall)
            {
                float force = Mathf.Lerp(minForce, maxForce, charge01);

                Vector2 dir = new Vector2(
                    Mathf.Cos(gateAngle * Mathf.Deg2Rad),
                    Mathf.Sin(gateAngle * Mathf.Deg2Rad)
                ).normalized;

                // Safety: ensure upward launch
                if (dir.y < 0f) dir.y = -dir.y;

                rb.velocity = Vector2.zero;
                rb.AddForce(dir * force, ForceMode2D.Impulse);
            }

            chargingOnWall = false;
            charge01 = 0f;
        }
    }

    // ---- Grounded check ----
    void OnCollisionStay2D(Collision2D col)
    {
        bool wasGrounded = grounded;
        grounded = false;
        for (int i = 0; i < col.contactCount; i++)
        {
            if (col.GetContact(i).normal.y > 0.5f)
            {
                grounded = true;
                if (!wasGrounded && landingVelocityY < -5f)
                    StartCoroutine(LandingSquash());
                break;
            }
        }
    }

    IEnumerator LandingSquash()
    {
        transform.localScale = new Vector3(
            originalScale.x * squashX,
            originalScale.y * squashY,
            originalScale.z);
        yield return new WaitForSeconds(landingSquashDuration);
        transform.localScale = originalScale;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        grounded = false;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        float radius = 1.2f;
        int segments = 24;
        Vector3 center = transform.position;

        // Draw allowed arc
        Gizmos.color = Color.cyan;
        Vector3 prev = center + AngleToDir(minAngle) * radius;
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            float a = Mathf.Lerp(minAngle, maxAngle, t);
            Vector3 next = center + AngleToDir(a) * radius;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }

        // Draw current aim pointer
        if (charging)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(center, center + AngleToDir(gateAngle) * radius);
        }
    }

    Vector3 AngleToDir(float angle)
    {
        return new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0f);
    }
#endif
}