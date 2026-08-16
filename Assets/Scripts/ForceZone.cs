using UnityEngine;

/// <summary>
/// Applies continuous force to bubbles inside the trigger.
/// Can represent a fan, toaster, steam, etc.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ForceZone : MonoBehaviour
{
    [Header("Force")]
    [SerializeField] private Vector2 forceDirection = Vector2.up;

    [SerializeField] private float forceStrength = 5f;

    [Header("Optional")]
    [SerializeField] private bool normalizeDirection = true;

    private Vector2 Force
    {
        get
        {
            Vector2 direction = forceDirection;

            if (normalizeDirection)
            {
                direction = direction.normalized;
            }

            return direction * forceStrength;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Bubble bubble =
            other.GetComponentInParent<Bubble>();

        if (bubble == null)
            return;

        bubble.Rigidbody.AddForce(
            Force,
            ForceMode2D.Force
        );
    }
}