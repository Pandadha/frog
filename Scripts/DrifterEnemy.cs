using UnityEngine;

public class DrifterEnemy : MonoBehaviour
{
    [Header("Patrol Path")]
    public Transform pointA;          // assign in Inspector
    public Transform pointB;          // assign in Inspector
    public float speed = 2.5f;        // units per second
    public float waitTime = 0.6f;     // pause at each end before turning

    [Header("Bob (natural feel)")]
    public float bobAmount = 0.18f;   // vertical sine drift
    public float bobSpeed = 1.8f;     // how fast it bobs

    [Header("Visual")]
    public Transform visual;          // the sprite/child to flip & bob
    public float wingFlapSpeed = 6f;  // if you animate wings via scale

    [Header("Detection (optional)")]
    public float awarenessRadius = 5f;     // just for future use / gizmos


    private Vector3 target;
   
    private float waitTimer;
    private bool waiting;
    private bool goingToB = true;
    private float bobOffset;
  

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning("DrifterEnemy: assign Point A and Point B in the Inspector.");
            enabled = false;
            return;
        }

        transform.position = pointA.position;
   
        target = pointB.position;
       

        // Random offset so multiple drifters don't sync up
        bobOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        // ---- Wait at waypoint ----
        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                // Swap target
                goingToB = !goingToB;
               
                target = goingToB ? pointB.position : pointA.position;
                FlipVisual();
            }
            ApplyBob();
            return;
        }

    
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        ApplyBob();

        // ---- Reached target? ----
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            transform.position = target;
            waiting = true;
            waitTimer = waitTime;
        }
    }

    void ApplyBob()
    {
        if (visual == null) return;

        float bob = Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobAmount;
        visual.localPosition = new Vector3(
            visual.localPosition.x,
            bob,
            visual.localPosition.z
        );

        // Optional: subtle wing flap via Y scale pulse
        float flap = 1f + Mathf.Abs(Mathf.Sin(Time.time * wingFlapSpeed)) * 0.08f;
        visual.localScale = new Vector3(
            Mathf.Abs(visual.localScale.x) * (goingToB ? 1f : -1f),
            flap,
            visual.localScale.z
        );
    }

    

    void FlipVisual()
    {
        if (visual == null) return;
        Vector3 s = visual.localScale;
        visual.localScale = new Vector3(-s.x, s.y, s.z);
    }
    void OnTriggerEnter2D(Collider2D other) => DamagePlayer(other);
    void OnTriggerStay2D(Collider2D other) => DamagePlayer(other);

    void DamagePlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerHealth>()?.TakeDamage(1, transform.position);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Patrol path line
        if (pointA != null && pointB != null)
        {
            Gizmos.color = new Color(0.4f, 1f, 0.6f, 0.8f);
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.2f);
            Gizmos.DrawWireSphere(pointB.position, 0.2f);
        }

        // Awareness radius
        Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.15f);
        Gizmos.DrawWireSphere(transform.position, awarenessRadius);
    }
#endif
}