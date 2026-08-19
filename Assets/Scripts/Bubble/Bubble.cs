using UnityEngine;

/// <summary>
/// Represents one playable bubble.
/// Large, Medium and Small bubbles all use this same component.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bubble : MonoBehaviour
{
    public enum BubbleSize
    {
        Large,
        Medium,
        Small
    }

    [Header("Bubble")]
    [SerializeField] private BubbleSize size = BubbleSize.Large;

    [Header("Physics")]
    [SerializeField] private float mass = 1f;
    [SerializeField] private float gravityScale = 0.5f;
    [SerializeField] private float linearDrag = 0.5f;
    [SerializeField] private float hazardHitCooldown = 0.25f;


    [Header("Forward Movement")]
    [Tooltip("Constant horizontal speed toward the sink.")]
    [SerializeField] private float forwardSpeed = 3f;

    [Tooltip("Maximum vertical velocity.")]
    [SerializeField] private float maxVerticalVelocity = 6f;


    [Header("Out Of Bounds")]
    [SerializeField] private float minimumY = -10f;
    [SerializeField] private float maximumY = 10f;


    private float hazardHitTimer;
    private Rigidbody2D rb;
    public BubbleSize Size => size;
    public Rigidbody2D Rigidbody => rb;
    public Vector2 Velocity => rb.velocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        ConfigurePhysics();
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterBubble(this);
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterBubble(this);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentState !=
            GameManager.GameState.Playing)
        {
            return;
        }

        hazardHitTimer -= Time.deltaTime;

        CheckOutOfBounds();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentState !=
            GameManager.GameState.Playing)
        {
            return;
        }

        MaintainForwardMovement();
        ClampVerticalVelocity();
    }

    private void ConfigurePhysics()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;

        rb.mass = mass;
        rb.gravityScale = gravityScale;
        rb.drag = linearDrag;

        rb.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        rb.interpolation =
            RigidbodyInterpolation2D.Interpolate;
    }

    /// <summary>
    /// Keeps the bubble constantly moving toward the sink.
    /// Vertical velocity is not changed here, so gravity and
    /// wind impulses can still affect the bubble naturally.
    /// </summary>
    private void MaintainForwardMovement()
    {
        Vector2 velocity = rb.velocity;

        velocity.x = forwardSpeed;

        rb.velocity = velocity;
    }

    private void ClampVerticalVelocity()
    {
        Vector2 velocity = rb.velocity;

        velocity.y = Mathf.Clamp(
            velocity.y,
            -maxVerticalVelocity,
            maxVerticalVelocity
        );

        rb.velocity = velocity;
    }

    private void CheckOutOfBounds()
    {
        if (transform.position.y < minimumY || transform.position.y > maximumY)
        {
            GameManager.Instance.HandleBubbleOutOfBounds(this);
        }
    }

    /// <summary>
    /// Applies an instantaneous impulse, used by mouse/tap wind.
    /// </summary>
    public void ApplyImpulse(Vector2 impulse)
    {
        if (rb == null)
            return;

        rb.AddForce(
            impulse,
            ForceMode2D.Impulse
        );
    }

    /// <summary>
    /// Used by GameManager when spawning split bubbles.
    /// </summary>
    public void SetInitialVelocity(Vector2 velocity)
    {
        if (rb == null)
            return;

        rb.velocity = velocity;

        // Make sure split bubbles still move forward.
        Vector2 currentVelocity = rb.velocity;
        currentVelocity.x = forwardSpeed;

        rb.velocity = currentVelocity;
    }

    public void HitHazard()
    {
        if (GameManager.Instance == null)
            return;
        
        if (hazardHitTimer > 0f)
            return;

        hazardHitTimer = hazardHitCooldown;
        GameAudio.Instance.PlayBubblePop();
        GameVFX.Instance.SpawnBubblePop(transform.position);
        GameManager.Instance.HandleBubbleHazardHit(this);
    }

    public void DisablePhysics()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;
    }
}